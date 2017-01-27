using System;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Abstract;
using Zone.Campaign.WebServices.Services.Responses;
using log4net;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the xtk:session SOAP services.
    /// </summary>
    public class SessionService : Service, IAuthenticationService
    {
        #region Fields

        private const string ServiceNamespace = "xtk:session";

        private static readonly ILog Log = LogManager.GetLogger(typeof(SessionService));

        private readonly ISoapRequestHandler _requestHandler;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="SessionService"/>
        /// </summary>
        /// <param name="requestHandler">Handler for the SOAP requests</param>
        public SessionService(ISoapRequestHandler requestHandler)
        {
            if (requestHandler == null)
            {
                throw new ArgumentNullException(nameof(requestHandler));
            }

            _requestHandler = requestHandler;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Authorise user using provided credentials and retrieve security and session tokens.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Security and session tokens</returns>
        public Response<Tokens> Logon(string username, string password)
        {
            const string serviceName = "Logon";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest("Logon", serviceNs, null);
            var requestDoc = serviceElement.OwnerDocument;

            // Build request for this service.
            serviceElement.AppendChildWithValue("urn:strLogin", serviceNs, username);
            serviceElement.AppendChildWithValue("urn:strPassword", serviceNs, password);
            serviceElement.AppendChild("urn:elemParameters", serviceNs);

            // Execute request and get response from server.
            var response = _requestHandler.ExecuteRequest(null, ServiceNamespace, serviceName, requestDoc);

                        Log.Debug($"Response to {serviceName} received: {response.Status}");

            if (!response.Success)
            {
                return new Response<Tokens>(response.Status, response.Message, response.Exception);
            }

            // Parse response to extract session and security tokens. It would be nice to deserialize this.
            // These should always be there - any unsuccessful response should be caught above.
            var sessionTokenNode = SelectSingleNode(response.Data, "urn:pstrSessionToken", serviceNs);
            var securityTokenNode = SelectSingleNode(response.Data, "urn:pstrSecurityToken", serviceNs);
        
            return new Response<Tokens>(ResponseStatus.Success, new Tokens(securityTokenNode.InnerText, sessionTokenNode.InnerText));
        }

        #endregion
    }
}
