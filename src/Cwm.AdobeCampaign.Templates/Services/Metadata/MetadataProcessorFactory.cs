#if NETSTANDARD2_0
using Microsoft.Extensions.Logging;
#endif

namespace Cwm.AdobeCampaign.Templates.Services.Metadata
{
    /// <summary>
    /// Contains functions to return metadata extractor and metadata inserter classes for given file types.
    /// </summary>
    public class MetadataProcessorFactory : IMetadataExtractorFactory, IMetadataInserterFactory
    {
        #region Fields

#if NETSTANDARD2_0
        private ILoggerFactory _loggerFactory;
#endif

        #endregion

        #region Constructor

#if NETSTANDARD2_0
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataProcessorFactory"/> class. 
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        public MetadataProcessorFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
#endif

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
