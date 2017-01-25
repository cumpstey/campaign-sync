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
    public class HttpSoapRequestHandler : ISoapRequestHandler
    {
        #region Methods

        public Response<XmlNode> ExecuteRequest(Uri uri, IEnumerable<string> customHeaders, Tokens tokens, string serviceName, string serviceNamespace, XmlDocument requestDoc)
        {
            return ExecuteRequest(uri, customHeaders, tokens, serviceName, serviceNamespace, requestDoc.InnerXml);
        }

        public Response<XmlNode> ExecuteRequest(Uri uri, IEnumerable<string> customHeaders, Tokens tokens, string serviceName, string serviceNamespace, string requestBody)
        {
            // Execute request and get response from server.
            string responseFromServer;
            try
            {
                responseFromServer = PerformHttpRequest(uri, customHeaders, tokens, serviceName, serviceNamespace, requestBody);
            }
            catch (WebException ex)
            {
                var httpResponse = ex.Response as HttpWebResponse;
                if (httpResponse == null)
                {
                    return new Response<XmlNode>(ResponseStatus.ConnectionError, ex.Message, ex);
                }

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.Forbidden:
                        return new Response<XmlNode>(ResponseStatus.Unauthorised, ex.Message, ex);
                    default:
                        return new Response<XmlNode>(ResponseStatus.UnknownError, ex.Message, ex);
                }
            }
            catch (Exception ex)
            {
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
                return new Response<XmlNode>(ResponseStatus.WebServiceError, string.Concat("Invalid xml returned: ", ex.Message), ex);
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
                return new Response<XmlNode>(ResponseStatus.WebServiceError, message);
            }
            else
            {
                return new Response<XmlNode>(ResponseStatus.Success, responseElement);
            }
        }

        #endregion

        #region Helpers

        private string PerformHttpRequest(Uri uri, IEnumerable<string> customHeaders, Tokens tokens, string serviceName, string serviceNamespace, string requestBody)
        {
            // We have done the login now we can actually do a query on Neolane
            var reqData = (HttpWebRequest)WebRequest.Create(uri);
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

            if (customHeaders != null)
            {
                foreach (var header in customHeaders.Where(i => Regex.IsMatch(i, @"^[a-z_-]:", RegexOptions.Compiled | RegexOptions.IgnoreCase)))
                {
                    reqData.Headers.Add(header);
                }
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
