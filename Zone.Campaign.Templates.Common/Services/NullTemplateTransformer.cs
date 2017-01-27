namespace Zone.Campaign.Templates.Services
{
    /// <summary>
    /// Dummy template transformer, which has no effect on the input.
    /// </summary>
    public class NullTemplateTransformer : ITemplateTransformer
    {
        #region Methods

        /// <summary>
        /// This makes no change to the input.
        /// </summary>
        /// <param name="input">Input content</param>
        /// <param name="workingDirectory">Directory in which the file being processed is stored</param>
        /// <returns>Transformed content (unchanged from input)</returns>
        public string Transform(string input, string workingDirectory)
        {
            return input;
        }

        #endregion
    }
}
