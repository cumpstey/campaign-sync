using System;

namespace Zone.Campaign.Templates.Services
{
    public class TemplateTransformerFactory : ITemplateTransformerFactory
    {
        #region Methods

        public ITemplateTransformer GetTransformer(string fileExtension)
        {
           switch (fileExtension)
           {
               case ".html":
                   return new HtmlTemplateTransformer();
               default:
                   return new NullTemplateTransformer();
           }
        }

        #endregion
    }
}
