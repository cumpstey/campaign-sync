namespace Zone.Campaign.Sync
{
    /// <summary>
    /// Defines whether to use standard services or zipped services.
    /// </summary>
    public enum RequestMode
    {
        /// <summary>
        /// Use standard services.
        /// </summary>
        Default,

        /// <summary>
        /// Use zipped services.
        /// </summary>
        Zip,
    }
}
