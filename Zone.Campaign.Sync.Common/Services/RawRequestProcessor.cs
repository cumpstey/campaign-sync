using System;
using System.IO;
using System.Text.RegularExpressions;
using log4net;
using Zone.Campaign.Templates.Exceptions;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.Templates.Services.Metadata;
using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.Services
{
    /// <summary>
    /// Contains methods to process a raw SOAP request stored in a file on disk.
    /// </summary>
    public class RawRequestProcessor : IRawRequestProcessor
    {
        #region Fields

        private static readonly Regex MetadataCommentRegex = new Regex("^!(?<value>.*)!$", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly ILog Log = LogManager.GetLogger(typeof(RawRequestProcessor));

        private readonly IXmlMetadataExtractor _metadataExtractor;

        #endregion

        #region Constructor

        public RawRequestProcessor(IXmlMetadataExtractor metadataExtractor)
        {
            if (metadataExtractor == null)
            {
                throw new ArgumentNullException(nameof(metadataExtractor));
            }

            _metadataExtractor = metadataExtractor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a raw SOAP request stored in a file on disk.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="filePath">Path to file containing raw request</param>
        public void ProcessRequest(IRequestHandler requestHandler, string filePath)
        {
            if (!File.Exists(filePath))
            {
                Log.Warn($"File doesn't exist at {filePath}.");
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
                Log.Warn($"Error parsing metadata from {filePath}", ex);
                return;
            }

            if (details.Metadata == null)
            {
                Log.Warn($"No metadata found in {filePath}");
                return;
            }

            ServiceName serviceName;
            if (!details.Metadata.AdditionalProperties.ContainsKey("Service")
                || !ServiceName.TryParse(details.Metadata.AdditionalProperties["Service"], out serviceName))
            {
                Log.Warn($"Incomplete SOAP service specification in {filePath}.");
                return;
            }

            requestHandler.ExecuteRequest(serviceName, details.Code);
        }

        #endregion
    }
}
