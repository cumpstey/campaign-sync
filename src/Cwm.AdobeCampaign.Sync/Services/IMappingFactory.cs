using Cwm.AdobeCampaign.Sync.Mappings.Abstract;

namespace Cwm.AdobeCampaign.Sync.Services
{
    /// <summary>
    /// Contains a function to return a mapping class for a given schema.
    /// </summary>
    public interface IMappingFactory
    {
        /// <summary>
        /// Returns a mapping class for a given schema.
        /// </summary>
        /// <param name="schema">Schema</param>
        /// <returns>Mapping class</returns>
        IMapping GetMapping(string schema);
    }
}
