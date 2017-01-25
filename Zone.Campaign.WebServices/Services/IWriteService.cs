using System;
using System.Collections.Generic;
using Zone.Campaign.WebServices.Model.Abstract;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    public interface IWriteService
    {
        #region Methods

        Response Write<T>(Uri uri, IEnumerable<string> customHeaders, Tokens tokens, T item)
            where T : IPersistable;

        Response WriteCollection<T>(Uri uri, IEnumerable<string> customHeaders, Tokens tokens, IEnumerable<T> items)
            where T : IPersistable;
        
        #endregion
    }
}
