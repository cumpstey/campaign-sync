using log4net;
using System;
using System.IO;
using System.Linq;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Services;
using Zone.Campaign.Templates.Services.Metadata;

namespace Zone.Campaign.Sync.Services
{
    /// <summary>
    /// Contains methods to download files from Campaign and save them to disk.
    /// </summary>
    public class Downloader : IDownloader
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(Downloader));

        private readonly IMappingFactory _mappingFactory;

        private readonly IMetadataInserterFactory _metadataInserterFactory;

        private readonly IQueryService _queryService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="Downloader"/>
        /// </summary>
        /// <param name="mappingFactory">Mapping factory</param>
        /// <param name="metadataInserterFactory">Metadata inserter factory</param>
        /// <param name="queryService">Query service</param>
        public Downloader(IMappingFactory mappingFactory,
                          IMetadataInserterFactory metadataInserterFactory,
                          IQueryService queryService)
        {
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
        public void DoDownload(IRequestHandler requestHandler, DownloadSettings settings)
        {
            // Create output dir
            var outDir = Path.Combine(settings.OutputDirectory, settings.Schema.Replace(":", "_"));

            // Get mapping for defined schema
            var mapping = _mappingFactory.GetMapping(settings.Schema);
            if (mapping == null)
            {
                Log.Error($"Unrecognised schema: {settings.Schema}");
                return;
            }

            // Do query
            var response = _queryService.ExecuteQuery(requestHandler, settings.Schema, mapping.QueryFields, settings.Conditions);

            if (!response.Success)
            {
                Log.Error($"Query failed: {response.Message}");
                return;
            }

            Log.Debug($"Query succeeded:{Environment.NewLine}{string.Join(",", response.Data)}");

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
                    Log.Error($"Query response parsing failed: {ex.Message}");
                    Log.Debug(ex);
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
                Log.Debug($"{template.Metadata.Name} downloaded.");
            }

            Log.Info($"{response.Data.Count()} files downloaded.");
        }

        #endregion
    }
}
