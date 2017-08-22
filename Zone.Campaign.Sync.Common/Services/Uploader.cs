using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using log4net;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.Templates.Services;
using Zone.Campaign.Templates.Services.Metadata;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.Services
{
    /// <summary>
    /// Contains methods to read files from disk and upload them into Campaign.
    /// </summary>
    public class Uploader : IUploader
    {
        #region Fields
        
        private static readonly ILog Log = LogManager.GetLogger(typeof(Uploader));

        private readonly IImageDataProvider _imageDataProvider;

        private readonly IMappingFactory _mappingFactory;

        private readonly IMetadataExtractorFactory _metadataExtractorFactory;

        ////private readonly ITemplateTransformerFactory _templateTransformerFactory;

        private readonly IBuilderService _builderService;

        private readonly IImageWriteService _imageWriteService;

        private readonly IWriteService _writeService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="Uploader"/>
        /// </summary>
        /// <param name="imageDataProvider">Image data provider</param>
        /// <param name="mappingFactory">Mapping factory</param>
        /// <param name="metadataExtractorFactory">Metadata extractor factory</param>
        /// <param name="templateTransformerFactory">Template transformer factory</param>
        /// <param name="builderService">Builder service</param>
        /// <param name="imageWriteService">Image write service</param>
        /// <param name="writeService">Write service</param>
        public Uploader(IImageDataProvider imageDataProvider,
                        IMappingFactory mappingFactory,
                        IMetadataExtractorFactory metadataExtractorFactory,
                        ////ITemplateTransformerFactory templateTransformerFactory,
                        IBuilderService builderService,
                        IImageWriteService imageWriteService,
                        IWriteService writeService)
        {
            _imageDataProvider = imageDataProvider;
            _mappingFactory = mappingFactory;
            _metadataExtractorFactory = metadataExtractorFactory;
            ////_templateTransformerFactory = templateTransformerFactory;
            _builderService = builderService;
            _imageWriteService = imageWriteService;
            _writeService = writeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Upload a set of files defined by the settings.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="settings">Upload settings</param>
        public void DoUpload(IRequestHandler requestHandler, UploadSettings settings)
        {
            var pathList = settings.FilePaths.SelectMany(i =>
            {
                if (File.Exists(i))
                {
                    return new[] { i };
                }

                if (Directory.Exists(i))
                {
                    return Directory.GetFiles(i, "*", SearchOption.AllDirectories);
                }

                var dir = Path.GetDirectoryName(i);
                if (Directory.Exists(dir))
                {
                    return Directory.GetFiles(dir, Path.GetFileName(i), SearchOption.AllDirectories);
                }

                Log.Warn($"{i} specified for upload but no matching files found.");
                return new string[0];
            }).ToArray();

            var templateList = pathList.Select(i =>
            {
                var fileExtension = Path.GetExtension(i);
                var metadataExtractor = _metadataExtractorFactory.GetExtractor(fileExtension);
                if (metadataExtractor == null)
                {
                    Log.Warn($"Unsupported filetype {i}.");
                    return null;
                }

                var raw = File.ReadAllText(i);
                var template = metadataExtractor.ExtractMetadata(raw);

                if (template.Metadata == null)
                {
                    Log.Warn($"No metadata found in {i}.");
                    return new Tuple<string, Template>(i, template);
                }
                else if (template.Metadata.Schema == null)
                {
                    Log.Warn($"No schema found in {i}.");
                    return new Tuple<string, Template>(i, template);
                }
                else if (template.Metadata.Name == null)
                {
                    Log.Warn($"No name found in {i}.");
                    return new Tuple<string, Template>(i, template);
                }

                return new Tuple<string, Template>(i, template);
            }).Where(i => i != null && i.Item2.Metadata != null && i.Item2.Metadata.Schema != null && i.Item2.Metadata.Name != null)
              .ToArray();

            if (settings.TestMode)
            {
                Log.Info($"{templateList.Count()} files found:{Environment.NewLine}{string.Join(Environment.NewLine, templateList.Select(i => i.Item2.Metadata.Name))}");
            }
            else
            {
                var templateTotalCount = templateList.Length;
                var templateProcessedCount = 0;
                var templateSuccessCount = 0;
                foreach (var item in templateList)
                {
                    var filePath = item.Item1;
                    var template = item.Item2;
                    templateProcessedCount++;

                    // Get mapping for defined schema, and generate object for write
                    var mapping = _mappingFactory.GetMapping(template.Metadata.Schema.ToString());

                    // Get transformer, which is dependent on file type as well as entity type
                    var templateTransformer = mapping.GetTransformer(Path.GetExtension(filePath));

                    // The transformer may return multiple variants, each of which should be uploaded
                    var variants = templateTransformer != null
                     ? templateTransformer.Transform(template, new TransformParameters {
                         OriginalFileName = filePath,
                         ApplyTransforms = settings.ApplyTransforms,
                     })
                     : new[] { template };

                    // Upload each variant
                    foreach (var variant in variants)
                    {
                        if (variant.Code != null && settings.Replacements != null)
                        {
                            foreach (var replacement in settings.Replacements)
                            {
                                variant.Code = variant.Code.Replace(replacement.Item1, replacement.Item2);
                            }
                        }

                        var persistable = mapping.GetPersistableItem(requestHandler, variant);

                        var response = _writeService.Write(requestHandler, persistable);
                        if (!response.Success)
                        {
                            Log.Warn($"Upload of {variant.Metadata.Name} failed: {response.Message}");
                        }
                        else
                        {
                            templateSuccessCount++;
                        }
                    }
                }

                Log.Info($"{templateSuccessCount} files uploaded.");

                // Build any schemas uploaded
                var schemaList = templateList.Where(i => i.Item2.Metadata.Schema.ToString() == Schema.EntitySchema).ToArray();
                if (schemaList.Any())
                {
                    var schemaTotalCount = schemaList.Length;
                    var schemaProcessedCount = 0;
                    var schemaSuccessCount = 0;
                    foreach (var item in schemaList)
                    {
                        var schema = item.Item2;
                        schemaProcessedCount++;

                        var response = _builderService.BuildSchema(requestHandler, schema.Metadata.Name);
                        if (!response.Success)
                        {
                            Log.Warn($"Build of {schema.Metadata.Name} failed: {response.Message}");
                        }
                        else
                        {
                            schemaSuccessCount++;
                        }
                    }

                    Log.Info($"{schemaSuccessCount} schemas built.");
                }

                // If any navigation hierarchies uploaded, build navigation hierarchy
                if (templateList.Any(i => i.Item2.Metadata.Schema.ToString() == NavigationHierarchy.EntitySchema))
                {
                    var response = _builderService.BuildNavigationHierarchy(requestHandler);
                    if (!response.Success)
                    {
                        Log.Warn($"Build of navigation hierarchy failed: {response.Message}");
                    }
                    else
                    {
                        Log.Info("Navigation hierarchy built.");
                    }
                }
            }
        }

        /// <summary>
        /// Upload a set of images defined by the settings.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="settings">Upload settings</param>
        public void DoImageUpload(IRequestHandler requestHandler, UploadSettings settings)
        {
            var imageData = settings.FilePaths.SelectMany(i => _imageDataProvider.GetData(i)).ToArray();

            var totalCount = imageData.Length;
            var processedCount = 0;
            var successCount = 0;
            foreach (var imageItem in imageData)
            {
                processedCount++;

                var mimeType = ImageHelper.GetMimeType(Path.GetExtension(imageItem.FilePath));
                if (mimeType == null)
                {
                    Log.Warn($"Unsupported file type: {imageItem.FilePath}");
                    continue;
                }

                var fileInfo = new FileInfo(imageItem.FilePath);
                if (!fileInfo.Exists)
                {
                    Log.Warn($"File does not exist: {imageItem.FilePath}");
                    continue;
                }

                InternalName internalName;
                if (!InternalName.TryParse(imageItem.InternalName, out internalName))
                {
                    Log.Warn($"Failed to parse internal name: {imageItem.InternalName}");
                    continue;
                }

                string md5Hash;
                using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(imageItem.FilePath))
                {
                    md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }

                int width, height;
                using (var bitmap = new Bitmap(imageItem.FilePath))
                {
                    width = bitmap.Width;
                    height = bitmap.Height;
                }

                var fileContent = Convert.ToBase64String(File.ReadAllBytes(imageItem.FilePath));

                var file = new ImageResource
                {
                    FolderName = imageItem.FolderName,
                    FileRes = new FileResource
                    {
                        Name = internalName,
                        Label = imageItem.Label,
                        Alt = imageItem.Alt,
                        Width = width,
                        Height = height,
                    },
                    FileName = fileInfo.Name,
                    MimeType = mimeType,
                    Md5 = md5Hash,
                    FileContent = fileContent,
                };

                var response = _imageWriteService.WriteImage(requestHandler, file);
                if (!response.Success)
                {
                    Log.Warn($"Upload of {imageItem.InternalName} failed: {response.Message}");
                }
                else
                {
                    successCount++;
                }
            }

            Log.Info($"{successCount} images uploaded.");
        }

        #endregion
    }
}
