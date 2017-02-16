namespace Zone.Campaign.Templates.Model
{
    /// <summary>
    /// Class representing the data stored in a file, corresponding to an item from Campaign.
    /// </summary>
    public class Template
    {
        #region Properties

        /// <summary>
        /// The code content of the file.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Metadata associated with the code, which is required for it to be uploaded to Campaign.
        /// </summary>
        public TemplateMetadata Metadata { get; set; }
        
        /// <summary>
        /// File extension.
        /// </summary>
        public string FileExtension { get; set; }

        #endregion
    }
}
