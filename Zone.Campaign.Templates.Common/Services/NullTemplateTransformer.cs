namespace Zone.Campaign.Templates.Services
{
    public class NullTemplateTransformer : ITemplateTransformer
    {
        #region Methods

        public string Transform(string input, string workingDirectory)
        {
            return input;
        }

        #endregion
    }
}
