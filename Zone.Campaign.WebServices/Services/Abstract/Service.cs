using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services.Abstract
{
    public abstract class Service
    {
        #region Fields

        protected const string SoapNs = "http://schemas.xmlsoap.org/soap/envelope/";

        #endregion

        #region Helpers

        protected XmlNode CreateServiceRequest(string serviceName, string serviceNs, Tokens tokens)
        {
            var doc = new XmlDocument();
            doc.LoadXml(string.Format(@"<soapenv:Envelope xmlns:soapenv=""{0}""/>", SoapNs));

            doc.DocumentElement.AppendChild("soapenv:Header", SoapNs);
            var bodyElement = doc.DocumentElement.AppendChild("soapenv:Body", SoapNs);

            var serviceElement = bodyElement.AppendChild(string.Concat("urn:", serviceName), serviceNs);
            if (tokens == null)
            {
                serviceElement.AppendChild("urn:sessiontoken", serviceNs);
            }
            else
            {
                serviceElement.AppendChildWithValue("urn:sessiontoken", serviceNs, tokens.SessionToken);
            }

            return serviceElement;
        }

        protected Response<XmlNode> ExecuteRequest(Uri rootUri, Tokens tokens, string serviceName, string serviceNamespace, XmlDocument requestDoc)
        {
            // Execute request and get response from server.
            string responseFromServer;
            try
            {
                responseFromServer = ExecuteRequest(rootUri, tokens, serviceName, serviceNamespace, requestDoc.InnerXml);
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
            nsmgr.AddNamespace("soap", SoapNs);

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

        private string ExecuteRequest(Uri rootUri, Tokens tokens, string serviceName, string serviceNamespace, string requestBody)
        {
            // We have done the login now we can actually do a query on Neolane
            var reqData = (HttpWebRequest)WebRequest.Create(new Uri(rootUri, "nl/jsp/soaprouter.jsp"));
            reqData.Method = "POST";
            reqData.ContentType = "text/xml; charset=utf-8";

            // Add to the headers the requested service action that we want to call
            reqData.Headers.Add("SOAPAction", string.Format("{0}#{1}", serviceNamespace, serviceName));

            // Add to the headers the security and session token
            // Tokens should be provided except for the login request
            if (tokens != null)
            {
                reqData.Headers.Add("X-Security-Token", tokens.SecurityToken);

                // The session token can be added either in the request body or as a cookie.
                // We're adding it in the body in CreateServiceRequest, so don't need it here.
                ////reqData.Headers.Add("cookie", "__sessiontoken=" + tokens.SessionToken);
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

        protected XmlNode SelectSingleNode(XmlNode node, string xpath, string serviceNs)
        {
            var nsmgr = new XmlNamespaceManager(node.OwnerDocument.NameTable);
            nsmgr.AddNamespace("soap", SoapNs);
            nsmgr.AddNamespace("urn", serviceNs);

            var responseElement = node.SelectSingleNode(xpath, nsmgr);
            return responseElement;
        }

        #endregion
    }
}
