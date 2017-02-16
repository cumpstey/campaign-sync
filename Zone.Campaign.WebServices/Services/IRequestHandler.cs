using System.Xml;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Handler to make requests to Campaign.
    /// </summary>
    public interface IRequestHandler
    {
        /// <summary>
        /// Execute a request.
        /// </summary>
        /// <param name="serviceName">Name of the SOAP service</param>
        /// <param name="requestDoc">SOAP content as XML document</param>
        /// <returns>Response status and content</returns>
        Response<XmlNode> ExecuteRequest(ServiceName serviceName, XmlDocument requestDoc);

        /// <summary>
        /// Execute a request.
        /// </summary>
        /// <param name="serviceName">Name of the SOAP service</param>
        /// <param name="requestBody">SOAP content as string</param>
        /// <returns>Response status and content</returns>
        Response<XmlNode> ExecuteRequest(ServiceName serviceName, string requestBody);
    }
}
