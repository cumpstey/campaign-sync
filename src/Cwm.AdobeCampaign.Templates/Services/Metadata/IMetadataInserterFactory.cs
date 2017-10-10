namespace Cwm.AdobeCampaign.Templates.Services.Metadata
{
    /// <summary>
    /// Contains a function to return a metadata inserter class for a given file type.
    /// </summary>
    public interface IMetadataInserterFactory
    {
        /// <summary>
        /// Returns a metadata inserter class for given file extension.
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Metadata inserter class</returns>
        IMetadataInserter GetInserter(string fileExtension);
    }
}
