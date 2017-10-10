using Cwm.AdobeCampaign.Templates.Model;

namespace Cwm.AdobeCampaign.Templates.Services.Metadata
{
    /// <summary>
    /// Provides functions for converting metadata and code into raw file content which can be written to disk.
    /// </summary>
    public interface IMetadataInserter
    {
        /// <summary>
        /// Converts metadata and code into raw file content which can be written to disk.
        /// </summary>
        /// <param name="input">Metadata and code</param>
        /// <returns>Raw file content</returns>
        string InsertMetadata(Template input);
    }
}
