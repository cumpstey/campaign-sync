using System;
using System.Collections.Generic;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
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
        /// <param name="tokens">Authentication tokens</param>
        /// <param name="schema">Schema of the data to query</param>
        /// <param name="fields">Fields to return</param>
        /// <param name="conditions">Conditions</param>
        /// <returns>Response containing collection of matching items</returns>
        Response<IEnumerable<string>> ExecuteQuery(Tokens tokens, string schema, IEnumerable<string> fields, IEnumerable<string> conditions);

        #endregion
    }
}
