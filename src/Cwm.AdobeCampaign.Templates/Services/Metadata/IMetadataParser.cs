using Cwm.AdobeCampaign.Templates.Model;

namespace Cwm.AdobeCampaign.Templates.Services.Metadata
{
    /// <summary>
    /// Provides a function for parsing metadata stored in files into a form which can be sent to Campaign.
    /// </summary>
    public interface IMetadataParser
    {
        /// <summary>
        /// Parse metadata taken from a file into a form which can be sent to Campaign.
        /// </summary>
        /// <param name="input">String representation of metadata</param>
        /// <returns>Metadata</returns>
        TemplateMetadata Parse(string input);
    }
}
