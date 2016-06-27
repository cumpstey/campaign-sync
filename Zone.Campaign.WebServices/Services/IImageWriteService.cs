using System;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    public interface IImageWriteService
    {
        #region Methods

        Response WriteImage(Uri rootUri, Tokens tokens, ImageFile item);
        
        #endregion
    }
}
