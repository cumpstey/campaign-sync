namespace Cwm.AdobeCampaign.WebServices.Security
{
    /// <summary>
    /// Holds the security token and session token required to authenticate Campaign requests.
    /// </summary>
    public class Tokens
    {
        #region Constructor

        /// <summary>
        /// Initializes an instance of the <see cref="Tokens"/> class, with a security token and a session token. 
        /// </summary>
        /// <param name="securityToken">Security token</param>
        /// <param name="sessionToken">Session token</param>
        public Tokens(string securityToken, string sessionToken)
        {
            SecurityToken = securityToken;
            SessionToken = sessionToken;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Security token.
        /// </summary>
        public string SecurityToken { get; private set; }

        /// <summary>
        /// Session token.
        /// </summary>
        public string SessionToken { get; private set; }

        #endregion
    }
}
