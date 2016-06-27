using System;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    public interface IBuilderService
    {
        #region Methods

        Response BuildSchema(Uri rootUri, Tokens tokens, InternalName schemaName);

        #endregion
    }
}
