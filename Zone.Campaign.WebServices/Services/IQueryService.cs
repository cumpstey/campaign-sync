using System;
using System.Collections.Generic;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    public interface IQueryService
    {
        #region Methods

        Response<IEnumerable<string>> ExecuteQuery(Uri uri, IEnumerable<string> customHeaders, Tokens tokens, string schema, IEnumerable<string> fields, IEnumerable<string> conditions);

        #endregion
    }
}
