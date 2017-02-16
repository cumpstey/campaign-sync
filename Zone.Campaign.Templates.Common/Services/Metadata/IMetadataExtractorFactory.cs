namespace Zone.Campaign.Templates.Services.Metadata
{
    /// <summary>
    /// Contains a function to return a metadata extractor class for a given file type.
    /// </summary>
    public interface IMetadataExtractorFactory
    {
        /// <summary>
        /// Returns a metadata extractor class for a given file type.
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Metadata extractor class</returns>
        IMetadataExtractor GetExtractor(string fileExtension);
    }
}
