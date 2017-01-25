using System;
using System.Collections.Generic;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    public interface IImageWriteService
    {
        #region Methods

        Response WriteImage(Uri uri, IEnumerable<string> customHeaders, Tokens tokens, ImageFile item);
        
        #endregion
    }
}
