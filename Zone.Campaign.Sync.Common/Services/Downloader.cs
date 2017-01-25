using log4net;
using System;
using System.IO;
using System.Linq;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.Templates.Services;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.Services
{
    public class Downloader : IDownloader
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(Downloader));

        private readonly IMappingFactory _mappingFactory;

        private readonly IMetadataInserterFactory _metadataInserterFactory;

        private readonly IQueryService _queryService;

        #endregion

        #region Constructor

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

        public void DoDownload(Uri uri, Tokens tokens, DownloadSettings settings)
        {
            // Create output dir
            var outDir = Path.Combine(settings.OutputDirectory, settings.Schema.Replace(":", "_"));

            // Get mapping for defined schema
            var mapping = _mappingFactory.GetMapping(settings.Schema);
            if (mapping == null)
            {
                Log.ErrorFormat("Unrecognised schema: {0}", settings.Schema);
                return;
            }

            // Do query
            var response = _queryService.ExecuteQuery(uri, settings.CustomHeaders, tokens, settings.Schema, mapping.QueryFields, settings.Conditions);

            if (!response.Success)
            {
                Log.ErrorFormat("Query failed: {0}", response.Message);
                return;
            }

            Log.DebugFormat("Query succeeded:{0}{1}", Environment.NewLine, string.Join(",", response.Data));

            foreach (var item in response.Data)
            {
                var rawCode = item.Replace(@" xmlns=""urn:xtk:queryDef""", string.Empty);
                Template template;
                try
                {
                    template = mapping.ParseQueryResponse(rawCode);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Query response parsing failed: {0}", ex.Message);
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
                Log.DebugFormat("{0} downloaded.", template.Metadata.Name);
            }

            Log.InfoFormat("{0} files downloaded.", response.Data.Count());
        }

        #endregion
    }
}
