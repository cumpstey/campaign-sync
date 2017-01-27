using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Handler to make SOAP requests over http.
    /// </summary>
    public class HttpSoapRequestHandler : ISoapRequestHandler
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="HttpSoapRequestHandler"/>
        /// </summary>
        /// <param name="uri">Uri of the SOAP handler</param>
        /// <param name="customHeaders">Headers to include in every request</param>
        public HttpSoapRequestHandler(Uri uri, IEnumerable<string> customHeaders)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            Uri = uri;
            CustomHeaders = customHeaders ?? new string[0];
        }

        #endregion

        #region Properties

        public Uri Uri { get; private set; }

        public IEnumerable<string> CustomHeaders { get; private set; }

        ////public Tokens AuthenticationTokens { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Execute a request by making an http SOAP request.
        /// </summary>
        /// <param name="tokens">Authentication tokens</param>
        /// <param name="serviceNamespace">Namespace of the SOAP service</param>
        /// <param name="serviceName">Name of the SOAP service</param>
        /// <param name="requestDoc">SOAP content as XML document</param>
        /// <returns>Response status and content</returns>
        public Response<XmlNode> ExecuteRequest(Tokens tokens, string serviceNamespace, string serviceName, XmlDocument requestDoc)
        {
            return ExecuteRequest(tokens, serviceNamespace, serviceName, requestDoc.InnerXml);
        }

        /// <summary>
        /// Execute a request by making an http SOAP request.
        /// </summary>
        /// <param name="tokens">Authentication tokens</param>
        /// <param name="serviceNamespace">Namespace of the SOAP service</param>
        /// <param name="serviceName">Name of the SOAP service</param>
        /// <param name="requestBody">SOAP content as string</param>
        /// <returns>Response status and content</returns>
        public Response<XmlNode> ExecuteRequest(Tokens tokens, string serviceNamespace, string serviceName, string requestBody)
        {
            // Execute request and get response from server.
            string responseFromServer;
            try
            {
                responseFromServer = PerformHttpRequest(tokens, serviceNamespace, serviceName, requestBody);
            }
            catch (WebException ex)
            {
                // Some errors we can translate into useful information.
                var httpResponse = ex.Response as HttpWebResponse;
                if (httpResponse == null)
                {
                    return new Response<XmlNode>(ResponseStatus.ConnectionError, ex.Message, ex);
                }

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new Response<XmlNode>(ResponseStatus.NotFound, ex.Message, ex);
                    case HttpStatusCode.Forbidden:
                        return new Response<XmlNode>(ResponseStatus.Unauthorised, ex.Message, ex);
                    default:
                        return new Response<XmlNode>(ResponseStatus.UnknownError, ex.Message, ex);
                }
            }
            catch (Exception ex)
            {
                // Any other error, we translate as 'unknown'.
                return new Response<XmlNode>(ResponseStatus.UnknownError, ex.Message);
            }

            // Check for bad xml or soap fault, which don't return an http exception.
            var responseDoc = new XmlDocument();
            try
            {
                responseDoc.LoadXml(responseFromServer);
            }
            catch (Exception ex)
            {
                // This is an unknown error because the server should always return valid xml.
                return new Response<XmlNode>(ResponseStatus.UnknownError, string.Concat("Invalid xml returned: ", ex.Message), ex);
            }

            var nsmgr = new XmlNamespaceManager(responseDoc.NameTable);
            nsmgr.AddNamespace("soap", Soap.XmlNamespace);

            var bodyElement = responseDoc.SelectSingleNode("/soap:Envelope/soap:Body", nsmgr);

            // Get first child of body. This will either be the response, or a fault.
            var responseElement = bodyElement.FirstChild;
            if (responseElement.LocalName == "Fault")
            {
                var messageNode = responseElement.SelectSingleNode("detail")
                    ?? responseElement.SelectSingleNode("faultstring");
                var message = messageNode != null
                    ? messageNode.InnerText
                    : "No error message returned";
                return new Response<XmlNode>(ResponseStatus.ProcessingError, message);
            }
            else
            {
                return new Response<XmlNode>(ResponseStatus.Success, responseElement);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Make the http request. This method does not handle any exceptions thrown by making the http request.
        /// </summary>
        private string PerformHttpRequest(Tokens tokens, string serviceNamespace, string serviceName, string requestBody)
        {
            // We have done the login now we can actually do a query on Neolane
            var reqData = (HttpWebRequest)WebRequest.Create(Uri);
            reqData.Method = "POST";
            reqData.ContentType = "text/xml; charset=utf-8";

            // Add to the headers the requested service action that we want to call
            reqData.Headers.Add("SOAPAction", string.Format("{0}#{1}", serviceNamespace, serviceName));

            // Add to the headers the security and session token
            // Tokens should be provided except for the login request
            if (tokens != null)
            {
                reqData.Headers.Add("X-Security-Token", tokens.SecurityToken);

                // The session token can be added either in the request body or in header as a cookie.
                // We add it as a header, to make it easier to deal with queuing requests for later processing.
                reqData.Headers.Add("cookie", "__sessiontoken=" + tokens.SessionToken);
            }

            foreach (var header in CustomHeaders.Where(i => Regex.IsMatch(i, @"^[a-z_-]+:", RegexOptions.Compiled | RegexOptions.IgnoreCase)))
            {
                reqData.Headers.Add(header);
            }

            // Write the body to a byteArray to be passed with the Request Stream
            var byteArrayData = Encoding.UTF8.GetBytes(requestBody);

            // Set the ContentLength property of the WebRequest.
            reqData.ContentLength = byteArrayData.Length;

            // Write the data to the request stream.
            using (var dataStreamInputData = reqData.GetRequestStream())
            {
                dataStreamInputData.Write(byteArrayData, 0, byteArrayData.Length);
            }

            // Get the response stream and open the stream using a StreamReader for easy access.
            string responseFromServer;
            using (var response = reqData.GetResponse())
            using (var dataStream = response.GetResponseStream())
            using (var reader = new StreamReader(dataStream))
            {
                // Read the content.
                responseFromServer = reader.ReadToEnd();
            }

            return responseFromServer;
        }

        #endregion
    }
}
