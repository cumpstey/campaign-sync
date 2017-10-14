using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Cwm.AdobeCampaign.Templates.Model;
using Cwm.AdobeCampaign.Templates.Services;
using Cwm.AdobeCampaign.Templates.Services.Metadata;
using Cwm.AdobeCampaign.WebServices.Model;
using Cwm.AdobeCampaign.WebServices.Services;
using System.Threading.Tasks;
using Cwm.AdobeCampaign.Templates.Services.Transforms;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace Cwm.AdobeCampaign.Sync.Services
{
    /// <summary>
    /// Contains methods to read files from disk and upload them into Campaign.
    /// </summary>
    public class Uploader : IUploader
    {
        #region Fields
        
        private readonly ILogger _logger;

        private readonly IImageDataProvider _imageDataProvider;

        private readonly IMappingFactory _mappingFactory;

        private readonly IMetadataExtractorFactory _metadataExtractorFactory;

        private readonly IBuilderService _builderService;

        private readonly IPublishingService _publishingService;

        private readonly IImageWriteService _imageWriteService;

        private readonly IWriteService _writeService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="Uploader"/>
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="imageDataProvider">Image data provider</param>
        /// <param name="mappingFactory">Mapping factory</param>
        /// <param name="metadataExtractorFactory">Metadata extractor factory</param>
        /// <param name="builderService">Builder service</param>
        /// <param name="publishingService">Publishing service</param>
        /// <param name="imageWriteService">Image write service</param>
        /// <param name="writeService">Write service</param>
        public Uploader(ILoggerFactory loggerFactory,
                        IImageDataProvider imageDataProvider,
                        IMappingFactory mappingFactory,
                        IMetadataExtractorFactory metadataExtractorFactory,
                        IBuilderService builderService,
                        IPublishingService publishingService,
                        IImageWriteService imageWriteService,
                        IWriteService writeService)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<Uploader>();
            _imageDataProvider = imageDataProvider;
            _mappingFactory = mappingFactory;
            _metadataExtractorFactory = metadataExtractorFactory;
            _builderService = builderService;
            _publishingService = publishingService;
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
        public async Task DoUploadAsync(IRequestHandler requestHandler, UploadSettings settings)
        {
            var pathList = settings.FilePaths?.SelectMany(i =>
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

                _logger.LogWarning($"{i} specified for upload but no matching files found.");
                return new string[0];
            }).ToArray() ?? new string[0];

            var templateList = pathList.Select(i =>
            {
                var fileExtension = Path.GetExtension(i);
                var metadataExtractor = _metadataExtractorFactory.GetExtractor(fileExtension);
                if (metadataExtractor == null)
                {
                    _logger.LogWarning($"Unsupported filetype {i}.");
                    return null;
                }

                var raw = File.ReadAllText(i);
                var template = metadataExtractor.ExtractMetadata(raw);

                if (template.Metadata == null)
                {
                    _logger.LogWarning($"No metadata found in {i}.");
                    return new Tuple<string, Template>(i, template);
                }
                else if (template.Metadata.Schema == null)
                {
                    _logger.LogWarning($"No schema found in {i}.");
                    return new Tuple<string, Template>(i, template);
                }
                else if (template.Metadata.Name == null)
                {
                    _logger.LogWarning($"No name found in {i}.");
                    return new Tuple<string, Template>(i, template);
                }

                return new Tuple<string, Template>(i, template);
            }).Where(i => i != null && i.Item2.Metadata != null && i.Item2.Metadata.Schema != null && i.Item2.Metadata.Name != null)
              .ToArray();

            if (settings.TestMode)
            {
                _logger.LogInformation($"{templateList.Count()} files found:{Environment.NewLine}{string.Join(Environment.NewLine, templateList.Select(i => i.Item2.Metadata.Name))}");
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

                        var response = await _writeService.WriteAsync(requestHandler, persistable);
                        if (!response.Success)
                        {
                            _logger.LogWarning($"Upload of {variant.Metadata.Name} failed: {response.Message}");
                        }
                        else
                        {
                            templateSuccessCount++;
                        }
                    }
                }

                _logger.LogInformation($"{templateSuccessCount} files uploaded.");

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

                        var response = await _builderService.BuildSchemaAsync(requestHandler, schema.Metadata.Name);
                        if (!response.Success)
                        {
                            _logger.LogWarning($"Build of {schema.Metadata.Name} failed: {response.Message}");
                        }
                        else
                        {
                            schemaSuccessCount++;
                        }
                    }

                    _logger.LogInformation($"{schemaSuccessCount} schemas built.");
                }

                // If any navigation hierarchies uploaded, build navigation hierarchy
                if (templateList.Any(i => i.Item2.Metadata.Schema.ToString() == NavigationHierarchy.EntitySchema))
                {
                    var response = await _builderService.BuildNavigationHierarchyAsync(requestHandler);
                    if (!response.Success)
                    {
                        _logger.LogWarning($"Build of navigation hierarchy failed: {response.Message}");
                    }
                    else
                    {
                        _logger.LogInformation("Navigation hierarchy built.");
                    }
                }

                //
                if (settings.PublishDeliveryTemplates)
                {
                    var response = await _publishingService.PublishTriggeredMessageInstancesAsync(requestHandler);
                    if (!response.Success)
                    {
                        _logger.LogWarning($"Publishing of Message Center services failed: {response.Message}");
                    }
                    else if (response.Data != null && response.Data.Any(i => !i.Value))
                    {
                        _logger.LogWarning($"Publishing of Message Center services failed for: {string.Join(", ", response.Data.Where(i => !i.Value).Select(i => i.Key))}");
                    }
                    else
                    {
                        _logger.LogInformation("Message Center services published.");
                    }
                }
            }
        }

        /// <summary>
        /// Upload a set of images defined by the settings.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="settings">Upload settings</param>
        public async Task DoImageUploadAsync(IRequestHandler requestHandler, UploadSettings settings)
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
                    _logger.LogWarning($"Unsupported file type: {imageItem.FilePath}");
                    continue;
                }

                var fileInfo = new FileInfo(imageItem.FilePath);
                if (!fileInfo.Exists)
                {
                    _logger.LogWarning($"File does not exist: {imageItem.FilePath}");
                    continue;
                }

                InternalName internalName;
                if (!InternalName.TryParse(imageItem.InternalName, out internalName))
                {
                    _logger.LogWarning($"Failed to parse internal name: {imageItem.InternalName}");
                    continue;
                }

                string md5Hash;
                using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(imageItem.FilePath))
                {
                    md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }

                int width, height;
                using (Image<Rgba32> image = Image.Load(imageItem.FilePath))
                {
                    width = image.Width;
                    height = image.Height;
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

                var response = await _imageWriteService.WriteImageAsync(requestHandler, file);
                if (!response.Success)
                {
                    _logger.LogWarning($"Upload of {imageItem.InternalName} failed: {response.Message}");
                }
                else
                {
                    successCount++;
                }
            }

            _logger.LogInformation($"{successCount} images uploaded.");
        }

        #endregion
    }
}
