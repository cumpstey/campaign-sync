using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Cwm.AdobeCampaign.WebServices.Security;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
#if NETSTANDARD2_0
using Microsoft.Extensions.Logging;
#endif

namespace Cwm.AdobeCampaign.WebServices.Services
{
    /// <summary>
    /// Handler to make SOAP requests over http.
    /// </summary>
    public class HttpSoapRequestHandler : IAuthenticatedRequestHandler
    {
        #region Fields

        /// TODO: abstract this, to allow testing
        private readonly HttpClient _client;

#if NETSTANDARD2_0
        private readonly ILogger _logger;
#endif

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="HttpSoapRequestHandler"/>
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="client">Http client</param>
        /// <param name="uri">Uri of the SOAP handler</param>
        /// <param name="customHeaders">Headers to include in every request</param>
#if NETSTANDARD2_0
        public HttpSoapRequestHandler(ILoggerFactory loggerFactory, HttpClient client, Uri uri, IEnumerable<string> customHeaders)
#else
        public HttpSoapRequestHandler(HttpClient client, Uri uri, IEnumerable<string> customHeaders)
#endif
        {
#if NETSTANDARD2_0
            _logger = loggerFactory.CreateLogger<HttpSoapRequestHandler>();
#endif
            _client = client;
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            CustomHeaders = customHeaders ?? new string[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Uri of the SOAP handler.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Collection of custom headers to send with each request, as "key: value".
        /// </summary>
        public IEnumerable<string> CustomHeaders { get; private set; }

        /// <summary>
        /// Authentication tokens.
        /// </summary>
        public Tokens AuthenticationTokens { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Execute a request by making an http SOAP request.
        /// </summary>
        /// <param name="serviceName">Name of the SOAP service</param>
        /// <param name="requestDoc">SOAP content as XML document</param>
        /// <returns>Response status and content</returns>
        public async Task<Response<XElement>> ExecuteRequestAsync(ServiceName serviceName, XDocument requestDoc)
        {
            return await ExecuteRequestAsync(serviceName, requestDoc.ToString());
        }

        /// <summary>
        /// Execute a request by making an http SOAP request.
        /// </summary>
        /// <param name="serviceName">Name of the SOAP service</param>
        /// <param name="requestBody">SOAP content as string</param>
        /// <returns>Response status and content</returns>
        public async Task<Response<XElement>> ExecuteRequestAsync(ServiceName serviceName, string requestBody)
        {
            // Execute request and get response from server.
            Tuple<HttpStatusCode, string, string> responseFromServer;
            try
            {
                responseFromServer = await PerformHttpRequestAsync(serviceName, requestBody);
            }
            catch (HttpRequestException ex)
            {
#if NETSTANDARD2_0
                _logger.LogError(ex, $"Error making soap request.");
#endif
                return new Response<XElement>(ResponseStatus.ConnectionError, ex.Message);
            }
            catch (Exception ex)
            {
                // Any error we translate as 'unknown', as this returns a status code.
#if NETSTANDARD2_0
                _logger.LogError(ex, $"Error making soap request.");
#endif
                return new Response<XElement>(ResponseStatus.UnknownError, ex.Message);
            }

            switch (responseFromServer.Item1)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.NotFound:
#if NETSTANDARD2_0
                    _logger.LogWarning($"{responseFromServer.Item1} response when making soap request.");
#endif
                    return new Response<XElement>(ResponseStatus.NotFound, responseFromServer.Item3);
                case HttpStatusCode.Forbidden:
#if NETSTANDARD2_0
                    _logger.LogWarning($"{responseFromServer.Item1} response when making soap request.");
#endif
                    return new Response<XElement>(ResponseStatus.Unauthorised, responseFromServer.Item3);
                default:
#if NETSTANDARD2_0
                    _logger.LogWarning($"{responseFromServer.Item1} response when making soap request.");
#endif
                    return new Response<XElement>(ResponseStatus.UnknownError, responseFromServer.Item3);
            }

            // Check for bad xml or soap fault, which don't return an http exception.
            XDocument responseDoc;
            try
            {
                responseDoc = XDocument.Parse(responseFromServer.Item2);
            }
            catch (Exception ex)
            {
#if NETSTANDARD2_0
                _logger.LogWarning(ex, $"Invalid xml returned from soap request.");
#endif

                // This is an unknown error because the server should always return valid xml.
                return new Response<XElement>(ResponseStatus.UnknownError, string.Concat("Invalid xml returned: ", ex.Message), ex);
            }

            XNamespace soapNs = Soap.XmlNamespace;
            var responseElement = responseDoc.Descendants(soapNs + "Body").First().Elements().First();

            if (responseElement.Name == soapNs + "Fault")
            {
                var message = (responseElement.Element("detail") ?? responseElement.Element("faultstring"))?.Value ?? "No error message returned";
#if NETSTANDARD2_0
                _logger.LogWarning($@"Fault response received: ""{message}"".");
#endif
                return new Response<XElement>(ResponseStatus.ProcessingError, message);
            }

            return new Response<XElement>(ResponseStatus.Success, responseElement);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Make the http request. This method does not handle any exceptions thrown by making the http request.
        /// </summary>
        private async Task<Tuple<HttpStatusCode, string, string>> PerformHttpRequestAsync(ServiceName serviceName, string requestBody)
        {
            var requestContent = new StringContent(requestBody, Encoding.UTF8, "text/xml");

            // Add to the headers the requested service action that we want to call
            requestContent.Headers.Add("SOAPAction", serviceName.ToString());

            // Add to the headers the security and session token
            // Tokens should be provided except for the login request
            if (AuthenticationTokens != null)
            {
                requestContent.Headers.Add("X-Security-Token", AuthenticationTokens.SecurityToken);

                // The session token can be added either in the request body or in header as a cookie.
                // We add it as a header, to make it easier to deal with queuing requests for later processing.
                requestContent.Headers.Add("cookie", $"__sessiontoken={AuthenticationTokens.SessionToken}");
            }

            // Add the customer headers
            // TODO: parse these at set time, rather than here
            foreach (var header in CustomHeaders)
            {
                var match = Regex.Match(header, @"^(?<name>[a-z_-]+):\s?(?<value>.*)$", RegexOptions.IgnoreCase);
                if (!match.Success)
                {
                    continue;
                }

                requestContent.Headers.Add(match.Groups["name"].Value, match.Groups["value"].Value);
            }

            var response = await _client.PostAsync(Uri, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            return new Tuple<HttpStatusCode, string, string>(response.StatusCode, responseContent, response.ReasonPhrase);
        }

        #endregion
    }
}
