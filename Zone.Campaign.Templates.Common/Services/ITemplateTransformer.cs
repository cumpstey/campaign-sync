namespace Zone.Campaign.Templates.Services
{
    public interface ITemplateTransformer
    {
        #region Methods

        string Transform(string input, string workingDirectory);

        #endregion
    }
}
