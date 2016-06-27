using System;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Abstract;
using Zone.Campaign.WebServices.Services.Responses;
using log4net;

namespace Zone.Campaign.WebServices.Services
{
    public class SessionService : Service, IAuthenticationService
    {
        #region Fields

        private const string ServiceNamespace = "xtk:session";

        private static readonly ILog Log = LogManager.GetLogger(typeof(SessionService));

        #endregion

        #region Methods

        /// <summary>
        /// Authorise user using provided credentials and retrieve security and session tokens.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Security and session tokens</returns>
        public Response<Tokens> Logon(Uri rootUri, string username, string password)
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
            var response = ExecuteRequest(rootUri, null, serviceName, ServiceNamespace, requestDoc);

            Log.DebugFormat("Response to {0} received: {1}", serviceName, response.Status);

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
