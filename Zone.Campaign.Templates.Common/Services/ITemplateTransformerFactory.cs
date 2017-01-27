namespace Zone.Campaign.Templates.Services
{
    /// <summary>
    /// Contains a function to return a template transformer class for a given file type.
    /// </summary>
    public interface ITemplateTransformerFactory
    {
        /// <summary>
        /// Returns a template transformer class for given file extension.
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Template transformer class</returns>
        ITemplateTransformer GetTransformer(string fileExtension);
    }
}
