using Cwm.AdobeCampaign.WebServices.Security;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using System.Threading.Tasks;

namespace Cwm.AdobeCampaign.WebServices.Services
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
        /// <param name="requestHandler">Request handler</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Security and session tokens</returns>
        Task<Response<Tokens>> LogonAsync(IRequestHandler requestHandler, string username, string password);
        
        #endregion
    }
}
