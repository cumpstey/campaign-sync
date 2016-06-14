using log4net;
using System;
using System.IO;
using System.Linq;
using Zone.Campaign.Templates.Services;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.Services
{
    public class Uploader
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(Uploader));

        private readonly IMappingFactory _mappingFactory;

        private readonly IMetadataExtractorFactory _metadataExtractorFactory;

        private readonly ITemplateTransformerFactory _templateTransformerFactory;

        #endregion

        #region Constructor

        public Uploader(IMappingFactory mappingFactory, IMetadataExtractorFactory metadataExtractorFactory, ITemplateTransformerFactory templateTransformerFactory)
        {
            _mappingFactory = mappingFactory;
            _metadataExtractorFactory = metadataExtractorFactory;
            _templateTransformerFactory = templateTransformerFactory;
        }

        #endregion

        #region Methods

        public void DoUpload(Uri rootUri, Tokens tokens, UploadSettings settings)
        {
            var persistService = new PersistService(rootUri);

            var pathList = settings.FilePaths.SelectMany(i =>
            {
                if (File.Exists(i))
                {
                    return new[] { i };
                }

                if (Directory.Exists(i))
                {
                    return Directory.GetFiles(i);
                }

                var dir = Path.GetDirectoryName(i);
                if (Directory.Exists(dir))
                {
                    return Directory.GetFiles(dir, Path.GetFileName(i));
                }

                Log.WarnFormat("{0} specified for upload but no matching files found.", i);
                return new string[0];
            });

            var templateList = pathList.Select(i =>
            {
                var fileExtension = Path.GetExtension(i);
                var metadataExtractor = _metadataExtractorFactory.GetExtractor(fileExtension);

                // TODO: I think maybe this should be set by the mapping, not the file extension.
                var templateTransformer = _templateTransformerFactory.GetTransformer(fileExtension);

                var raw = File.ReadAllText(i);
                var template = metadataExtractor.ExtractMetadata(raw);
                var workingDirectory = Path.GetDirectoryName(i);
                template.Code = templateTransformer.Transform(template.Code, workingDirectory);
                if (template.Metadata == null)
                {
                    Log.WarnFormat("No metadata found in {0}.", i);
                }
                else if (template.Metadata.Schema == null)
                {
                    Log.WarnFormat("No schema found in {0}.", i);
                }

                return template;
            }).Where(i => i.Metadata != null && i.Metadata.Schema != null);

            if (settings.TestMode)
            {
                Log.InfoFormat("{1} files found:{0}{2}", Environment.NewLine, templateList.Count(), string.Join(Environment.NewLine, templateList.Select(i => i.Metadata.Name)));
            }
            else
            {
                var count = 0;
                foreach (var template in templateList)
                {
                    // Get mapping for defined schema, and generate object for write
                    var mapping = _mappingFactory.GetMapping(template.Metadata.Schema.ToString());
                    var persistable = mapping.GetPersistableItem(template);

                    var response = persistService.Write(tokens, persistable);
                    if (!response.Success)
                    {
                        Log.WarnFormat("Upload of {0} failed: {1}", template.Metadata.Name, response.Message);
                    }
                    else
                    {
                        count++;
                    }
                }

                Log.InfoFormat("{0} files uploaded.", count);
            }
        }

        #endregion
    }
}
