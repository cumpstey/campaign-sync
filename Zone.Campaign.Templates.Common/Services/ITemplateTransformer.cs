namespace Zone.Campaign.Templates.Services
{
    /// <summary>
    /// Provides functions to transform code before it's uploaded to Campaign.
    /// </summary>
    public interface ITemplateTransformer
    {
        #region Methods

        /// <summary>
        /// Transforms code so that it can be uploaded to Campaign.
        /// Allows the content of local files to be different from that stored in Campaign.
        /// There is no reverse method, so file types which are transformed cannot be directly downloaded from Campaign
        /// into the format in which they are stored locally.
        /// </summary>
        /// <param name="input">Input content</param>
        /// <param name="workingDirectory">Directory in which the file being processed is stored</param>
        /// <returns>Transformed content</returns>
        string Transform(string input, string workingDirectory);

        #endregion
    }
}
