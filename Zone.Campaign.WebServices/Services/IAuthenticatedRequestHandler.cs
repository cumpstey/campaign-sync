using Zone.Campaign.WebServices.Security;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Handler to make authenticated requests to Campaign.
    /// </summary>
    public interface IAuthenticatedRequestHandler : IRequestHandler
    {
        /// <summary>
        /// Authentication tokens.
        /// </summary>
        Tokens AuthenticationTokens { get; set; }
    }
}
