using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Contains authentication functions.
    /// </summary>
    public interface IAuthenticationService
    {
        #region Methods

        /// <summary>
        /// Authorise user using provided credentials and retrieve security and session tokens.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Security and session tokens</returns>
        Response<Tokens> Logon(string username, string password);
        
        #endregion
    }
}
