using Microsoft.Extensions.Logging;

namespace Cwm.AdobeCampaign.Templates.Services.Metadata
{
    /// <summary>
    /// Contains functions to return metadata extractor and metadata inserter classes for given file types.
    /// </summary>
    public class MetadataProcessorFactory : IMetadataExtractorFactory, IMetadataInserterFactory
    {
        #region Fields

        private ILoggerFactory _loggerFactory;

        #endregion

        #region Constructor

        public MetadataProcessorFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a metadata extractor class for a given file type.
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Metadata extractor class</returns>
        public IMetadataExtractor GetExtractor(string fileExtension)
        {
            var metadataProcessor = new MetadataProcessor();
            switch (fileExtension)
            {
                case FileTypes.Html:
                    return new HtmlMetadataProcessor(metadataProcessor, metadataProcessor);
                case FileTypes.JavaScript:
                    return new JavaScriptMetadataProcessor(metadataProcessor, metadataProcessor);
                case FileTypes.Jssp:
                    return new JsspMetadataProcessor(metadataProcessor, metadataProcessor);
                case FileTypes.PlainText:
                    return new PlainTextMetadataProcessor(metadataProcessor, metadataProcessor);
                case FileTypes.Xml:
                    return new XmlMetadataProcessor(metadataProcessor, metadataProcessor);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns a metadata inserter class for given file extension.
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Metadata inserter class</returns>
        public IMetadataInserter GetInserter(string fileExtension)
        {
            var metadataProcessor = new MetadataProcessor();
            switch (fileExtension)
            {
                case FileTypes.Html:
                    return new HtmlMetadataProcessor(metadataProcessor, metadataProcessor);
                case FileTypes.JavaScript:
                    return new JavaScriptMetadataProcessor(metadataProcessor, metadataProcessor);
                case FileTypes.Jssp:
                    return new JsspMetadataProcessor(metadataProcessor, metadataProcessor);
                case FileTypes.PlainText:
                    return new PlainTextMetadataProcessor(metadataProcessor, metadataProcessor);
                case FileTypes.Xml:
                    return new XmlMetadataProcessor(metadataProcessor, metadataProcessor);
                default:
                    return null;
            }
        }

        #endregion
    }
}
