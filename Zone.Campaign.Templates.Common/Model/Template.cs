namespace Zone.Campaign.Templates.Model
{
    public class Template
    {
        #region Properties

        public string Code { get; set; }

        public TemplateMetadata Metadata { get; set; }
        
        public string FileExtension { get; set; }

        #endregion
    }
}
