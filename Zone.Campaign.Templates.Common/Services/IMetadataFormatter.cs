using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services
{
    /// <summary>
    /// Provides a function for transforming metadata sent from Campaign into a format in which it can be stored locally.
    /// </summary>
    public interface IMetadataFormatter
    {
        /// <summary>
        /// Format metadata sent from Campaign into a format in which it can be stored locally in a file on disk.
        /// </summary>
        /// <param name="metadata">Metadata</param>
        /// <returns>String representation of the metadata</returns>
        string Format(TemplateMetadata metadata);
    }
}
