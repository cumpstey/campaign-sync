namespace Zone.Campaign.WebServices.Security
{
    public class Tokens
    {
        #region Constructor

        public Tokens(string securityToken, string sessionToken)
        {
            SecurityToken = securityToken;
            SessionToken = sessionToken;
        }

        #endregion

        #region Properties

        public string SecurityToken { get; private set; }

        public string SessionToken { get; private set; }

        #endregion
    }
}
