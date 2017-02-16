namespace Zone.Campaign.Templates.Services
{
    /// <summary>
    /// Contains a function to return a template transformer class for a given file type.
    /// </summary>
    public class TemplateTransformerFactory : ITemplateTransformerFactory
    {
        #region Methods

        /// <summary>
        /// Returns a template transformer class for given file extension.
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Template transformer class</returns>
        public ITemplateTransformer GetTransformer(string fileExtension)
        {
           switch (fileExtension)
           {
               case FileTypes.Html:
                   return new HtmlTemplateTransformer();
                case FileTypes.Jssp:
                    return new JsspTemplateTransformer();
               default:
                   return new NullTemplateTransformer();
           }
        }

        #endregion
    }
}
