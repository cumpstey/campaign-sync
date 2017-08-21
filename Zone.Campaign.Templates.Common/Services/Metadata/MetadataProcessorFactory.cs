namespace Zone.Campaign.Templates.Services.Metadata
{
    /// <summary>
    /// Contains functions to return metadata extractor and metadata inserter classes for given file types.
    /// </summary>
    public class MetadataProcessorFactory : IMetadataExtractorFactory, IMetadataInserterFactory
    {
        #region Methods

        /// <summary>
        /// Returns a metadata extractor class for a given file type.
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Metadata extractor class</returns>
        public IMetadataExtractor GetExtractor(string fileExtension)
        {
            switch (fileExtension)
            {
                case FileTypes.Html:
                    return new HtmlMetadataProcessor();
                case FileTypes.JavaScript:
                    return new JavaScriptMetadataProcessor();
                case FileTypes.Jssp:
                    return new JsspMetadataProcessor();
                case FileTypes.PlainText:
                    return new PlainTextMetadataProcessor();
                case FileTypes.Xml:
                    return new XmlMetadataProcessor();
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
            switch (fileExtension)
            {
                case FileTypes.Html:
                    return new HtmlMetadataProcessor();
                case FileTypes.JavaScript:
                    return new JavaScriptMetadataProcessor();
                case FileTypes.Jssp:
                    return new JsspMetadataProcessor();
                case FileTypes.PlainText:
                    return new PlainTextMetadataProcessor();
                case FileTypes.Xml:
                    return new XmlMetadataProcessor();
                default:
                    return null;
            }
        }

        #endregion
    }
}
