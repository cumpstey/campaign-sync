using System;

namespace Zone.Campaign.Templates.Services
{
    public class MetadataProcessorFactory : IMetadataExtractorFactory, IMetadataInserterFactory
    {
        #region Methods

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
                case FileTypes.Xml:
                    return new XmlMetadataProcessor();
                default:
                    throw new InvalidOperationException("Unrecognised file extension.");
            }
        }

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
                case FileTypes.Xml:
                    return new XmlMetadataProcessor();
                default:
                    throw new InvalidOperationException("Unrecognised file extension.");
            }
        }

        #endregion
    }
}
