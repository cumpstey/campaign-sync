using System.Collections.Generic;
using Cwm.AdobeCampaign.WebServices.Services.Responses;

namespace Cwm.AdobeCampaign.WebServices.Services
{
    /// <summary>
    /// Contains query functions.
    /// </summary>
    public interface IQueryService
    {
        #region Methods

        /// <summary>
        /// Query the data based on a set of conditions.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="schema">Schema of the data to query</param>
        /// <param name="fields">Fields to return</param>
        /// <param name="conditions">Conditions</param>
        /// <returns>Response containing collection of matching items</returns>
        Response<IEnumerable<string>> ExecuteQuery(IRequestHandler requestHandler, string schema, IEnumerable<string> fields, IEnumerable<string> conditions);

        #endregion
    }
}
