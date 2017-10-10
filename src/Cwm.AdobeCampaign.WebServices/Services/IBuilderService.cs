using System.Threading.Tasks;
using Cwm.AdobeCampaign.WebServices.Services.Responses;

namespace Cwm.AdobeCampaign.WebServices.Services
{
    /// <summary>
    /// Contains schema builder functions.
    /// </summary>
    public interface IBuilderService
    {
        #region Methods

        /// <summary>
        /// Trigger a build of the entire navigation hierarchy.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <returns>Response</returns>
        Task<Response> BuildNavigationHierarchyAsync(IRequestHandler requestHandler);

        /// <summary>
        /// Trigger a build of the schema from the srcSchema.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="schemaName">Name of the schema to build</param>
        /// <returns>Response</returns>
        Task<Response> BuildSchemaAsync(IRequestHandler requestHandler, InternalName schemaName);

        #endregion
    }
}
