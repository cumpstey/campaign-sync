using System;
using System.Collections.Generic;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    public interface IBuilderService
    {
        #region Methods

        Response BuildSchema(Uri uri, IEnumerable<string> customHeaders, Tokens tokens, InternalName schemaName);

        #endregion
    }
}
