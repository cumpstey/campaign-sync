using System;
using System.Collections.Generic;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Contains image upload functions.
    /// </summary>
    public interface IImageWriteService
    {
        #region Methods

        /// <summary>
        /// Upload an image and create/update an xtk:fileRes record.
        /// </summary>
        /// <param name="tokens">Authenication tokens</param>
        /// <param name="item">Image file and metadata</param>
        /// <returns>Response</returns>
        Response WriteImage(Tokens tokens, ImageFile item);
        
        #endregion
    }
}
