using System;
using System.IO;
using System.Linq;
using Cwm.AdobeCampaign.Templates.Model;
using Cwm.AdobeCampaign.WebServices.Services;
using Cwm.AdobeCampaign.Templates.Services.Metadata;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Cwm.AdobeCampaign.Sync.Services
{
    /// <summary>
    /// Contains methods to download files from Campaign and save them to disk.
    /// </summary>
    public class Downloader : IDownloader
    {
        #region Fields

        private readonly ILogger _logger;

        private readonly IMappingFactory _mappingFactory;

        private readonly IMetadataInserterFactory _metadataInserterFactory;

        private readonly IQueryService _queryService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="Downloader"/>
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="mappingFactory">Mapping factory</param>
        /// <param name="metadataInserterFactory">Metadata inserter factory</param>
        /// <param name="queryService">Query service</param>
        public Downloader(ILoggerFactory loggerFactory,
                          IMappingFactory mappingFactory,
                          IMetadataInserterFactory metadataInserterFactory,
                          IQueryService queryService)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<Downloader>();
            _mappingFactory = mappingFactory;
            _metadataInserterFactory = metadataInserterFactory;
            _queryService = queryService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Download files based on parameters defined in the settings.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="settings">Download settings</param>
        public async Task DoDownloadAsync(IRequestHandler requestHandler, DownloadSettings settings)
        {
            // Create output dir
            var outDir = Path.Combine(settings.OutputDirectory, settings.Schema.Replace(":", "_"));

            // Get mapping for defined schema
            var mapping = _mappingFactory.GetMapping(settings.Schema);
            if (mapping == null)
            {
                _logger.LogWarning($"Unrecognised schema: {settings.Schema}");
                return;
            }

            // Do query
            var response = await _queryService.QueryAsync(requestHandler, settings.Schema, mapping.QueryFields, settings.Conditions);

            if (!response.Success)
            {
                _logger.LogError($"Query failed: {response.Message}");
                return;
            }

            _logger.LogDebug($"Query succeeded:{Environment.NewLine}{string.Join(",", response.Data)}");

            foreach (var item in response.Data)
            {
                var rawCode = item.Replace(@" xmlns=""urn:xtk:queryDef""", string.Empty);
                Template template;
                try
                {
                    template = mapping.ParseQueryResponse(requestHandler, rawCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Query response parsing failed.");
                    continue;
                }

                var metadataInserter = _metadataInserterFactory.GetInserter(template.FileExtension);
                var code = metadataInserter.InsertMetadata(template);

                var relativePath = settings.SubdirectoryMode == SubdirectoryMode.UnderscoreDelimited
                    ? template.Metadata.Name.Name.Replace("_", @"\")
                    : template.Metadata.Name.Name;
                var filePath = template.Metadata.Name.HasNamespace
                    ? Path.Combine(outDir, template.Metadata.Name.Namespace, relativePath)
                    : Path.Combine(outDir, relativePath);
                if (!filePath.EndsWith(template.FileExtension))
                {
                    filePath += template.FileExtension;
                }

                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(filePath, code);
                _logger.LogDebug($"{template.Metadata.Name} downloaded.");
            }

            _logger.LogInformation($"{response.Data.Count()} files downloaded.");
        }

        #endregion
    }
}
