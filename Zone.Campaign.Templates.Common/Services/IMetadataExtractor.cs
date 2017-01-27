using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services
{
    /// <summary>
    /// Provides functions for extracting metadata from files.
    /// </summary>
    public interface IMetadataExtractor
    {
        /// <summary>
        /// Extract the code and metadata from raw file content.
        /// </summary>
        /// <param name="input">Raw file content</param>
        /// <returns>Class containing code content and metadata.</returns>
        Template ExtractMetadata(string input);
    }
}
