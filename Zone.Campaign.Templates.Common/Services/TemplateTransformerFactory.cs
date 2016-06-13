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
               case FileTypes.Html:
                   return new HtmlTemplateTransformer();
               default:
                   return new NullTemplateTransformer();
           }
        }

        #endregion
    }
}
