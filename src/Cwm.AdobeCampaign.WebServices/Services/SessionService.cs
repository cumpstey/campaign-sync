using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cwm.AdobeCampaign.WebServices.Security;
using Cwm.AdobeCampaign.WebServices.Services.Abstract;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
#if NETSTANDARD2_0
using Microsoft.Extensions.Logging;
#endif

namespace Cwm.AdobeCampaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the xtk:session SOAP services.
    /// </summary>
    public class SessionService : Service, IAuthenticationService
    {
        #region Fields

        /// <summary>
        /// Soap namespace of the session services.
        /// </summary>
        public const string ServiceNamespace = "xtk:session";

        /// <summary>
        /// Soap name of the logon service.
        /// </summary>
        public const string LogonServiceName = "Logon";

#if NETSTANDARD2_0
        private readonly ILogger _logger;
#endif

        #endregion

        #region Constructor

#if NETSTANDARD2_0
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionService"/> class. 
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        public SessionService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SessionService>();
        }
#endif

        #endregion

        #region Methods

        /// <summary>
        /// Authorise user using provided credentials and retrieve security and session tokens.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Security and session tokens</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="requestHandler"/> is null.</exception>
        public async Task<Response<Tokens>> LogonAsync(IRequestHandler requestHandler, string username, string password)
        {
            if (requestHandler == null) throw new ArgumentNullException(nameof(requestHandler));

            XNamespace serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var requestDoc = CreateServiceRequest(LogonServiceName, serviceNs);
            var serviceElement = GetServiceElement(requestDoc, LogonServiceName, serviceNs);

            // Build request for this service.
            serviceElement.Add(new XElement(serviceNs + "strLogin", username),
                               new XElement(serviceNs + "strPassword", password),
                               new XElement(serviceNs + "elemParameters"));

            // Execute request and get response from server.
            var response = await requestHandler.ExecuteRequestAsync(new ServiceName(ServiceNamespace, LogonServiceName), requestDoc);

#if NETSTANDARD2_0
            _logger.LogDebug($"Response to {LogonServiceName} received: {response.Status}");
#endif

            if (!response.Success)
            {
                return new Response<Tokens>(response.Status, response.Message, response.Exception);
            }

            // Parse response to extract session and security tokens.
            // These should always be there - any unsuccessful response should be caught above.
            try
            {
                var sessionToken = response.Data.Element(serviceNs + "pstrSessionToken").Value;
                var securityToken = response.Data.Element(serviceNs + "pstrSecurityToken").Value;
                return new Response<Tokens>(ResponseStatus.Success, new Tokens(securityToken, sessionToken));
            }
            catch (Exception ex)
            {
#if NETSTANDARD2_0
                _logger.LogError(ex, $"Error parsing {LogonServiceName} response.");
#endif
                return new Response<Tokens>(ResponseStatus.ParsingError, $"Failed to parse {LogonServiceName} response", ex);
            }
        }

        #endregion
    }
}
