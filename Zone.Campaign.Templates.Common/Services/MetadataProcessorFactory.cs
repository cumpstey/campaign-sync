using System;

namespace Zone.Campaign.Templates.Services
{
    public class MetadataProcessorFactory : IMetadataExtractorFactory
    {
        #region Methods

       public IMetadataExtractor GetExtractor(string fileExtension)
        {
           switch (fileExtension)
           {
               case ".html":
                   return new HtmlMetadataProcessor();
               case ".js":
                   return new JavaScriptMetadataProcessor();
               case ".xml":
                   return new XmlMetadataProcessor();
               default:
                   throw new InvalidOperationException("Unrecognised file extension.");
           }
        }

        #endregion
    }
}
