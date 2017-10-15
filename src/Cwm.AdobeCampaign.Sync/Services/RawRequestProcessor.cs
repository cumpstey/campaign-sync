using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cwm.AdobeCampaign.Templates.Exceptions;
using Cwm.AdobeCampaign.Templates.Model;
using Cwm.AdobeCampaign.Templates.Services.Metadata;
using Cwm.AdobeCampaign.WebServices.Services;
using Microsoft.Extensions.Logging;

namespace Cwm.AdobeCampaign.Sync.Services
{
    /// <summary>
    /// Contains methods to process a raw SOAP request stored in a file on disk.
    /// </summary>
    public class RawRequestProcessor : IRawRequestProcessor
    {
        #region Fields

        private static readonly Regex MetadataCommentRegex = new Regex("^!(?<value>.*)!$", RegexOptions.Singleline);

        private readonly ILogger _logger;

        private readonly IXmlMetadataExtractor _metadataExtractor;

        #endregion

        #region Constructor

        public RawRequestProcessor(ILoggerFactory loggerFactory, IXmlMetadataExtractor metadataExtractor)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<RawRequestProcessor>();
            _metadataExtractor = metadataExtractor ?? throw new ArgumentNullException(nameof(metadataExtractor));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a raw SOAP request stored in a file on disk.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="filePath">Path to file containing raw request</param>
        public async Task ProcessRequestAsync(IRequestHandler requestHandler, string filePath)
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"File doesn't exist at {filePath}.");
                return;
            }

            var fileContent = File.ReadAllText(filePath);
            Template details;
            try
            {
                details = _metadataExtractor.ExtractMetadata(fileContent);
            }
            catch (MetadataException ex)
            {
                _logger.LogWarning(ex, $"Error parsing metadata from {filePath}");
                return;
            }

            if (details.Metadata == null)
            {
                _logger.LogWarning($"No metadata found in {filePath}");
                return;
            }

            ServiceName serviceName;
            if (!details.Metadata.AdditionalProperties.ContainsKey("Service")
                || !ServiceName.TryParse(details.Metadata.AdditionalProperties["Service"], out serviceName))
            {
                _logger.LogWarning($"Incomplete SOAP service specification in {filePath}.");
                return;
            }

            await requestHandler.ExecuteRequestAsync(serviceName, details.Code);
        }

        #endregion
    }
}
