﻿using System;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    public interface IAuthenticationService
    {
        #region Methods

        /// <summary>
        /// Authorise user using provided credentials and retrieve security and session tokens.
        /// </summary>
        /// <param name="rootUri">Root uri</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Security and session tokens</returns>
        Response<Tokens> Logon(Uri rootUri, string username, string password);
        
        #endregion
    }
}
