namespace Zone.Campaign.Sync
{
    /// <summary>
    /// Defines how to create a subdirectory structure when downloading files.
    /// </summary>
    public enum SubdirectoryMode
    {
        /// <summary>
        /// All files are put in a single folder.
        /// </summary>
        Default,

        /// <summary>
        /// Subdirectories are created based on underscore-delimited internal names.
        /// </summary>
        UnderscoreDelimited,
    }
}
