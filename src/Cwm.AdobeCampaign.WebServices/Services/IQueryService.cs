using System.Collections.Generic;
using System.Threading.Tasks;
using Cwm.AdobeCampaign.WebServices.Model.Abstract;
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
        Task<Response<IEnumerable<string>>> QueryAsync(IRequestHandler requestHandler, string schema, IEnumerable<string> fields, IEnumerable<string> conditions);

        /// <summary>
        /// Query the data based on a set of conditions.
        /// </summary>
        /// <typeparam name="T">Type of the data to query</typeparam>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="conditions">Conditions</param>
        /// <returns>Response containing collection of matching items</returns>
        Task<Response<IEnumerable<T>>> QueryAsync<T>(IRequestHandler requestHandler, IEnumerable<string> conditions) where T : IQueryable;

        #endregion
    }
}
