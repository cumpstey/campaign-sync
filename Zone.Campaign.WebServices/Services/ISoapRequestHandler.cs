using System.Xml;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Handler to make SOAP requests.
    /// </summary>
    public interface ISoapRequestHandler
    {
        /// <summary>
        /// Execute a request.
        /// </summary>
        /// <param name="tokens">Authentication tokens</param>
        /// <param name="serviceNamespace">Namespace of the SOAP service</param>
        /// <param name="serviceName">Name of the SOAP service</param>
        /// <param name="requestDoc">SOAP content as XML document</param>
        /// <returns>Response status and content</returns>
        Response<XmlNode> ExecuteRequest(Tokens tokens, string serviceNamespace, string serviceName, XmlDocument requestDoc);
    }
}
