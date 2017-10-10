using System.Xml;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cwm.AdobeCampaign.WebServices.Services
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
        Task<Response<XElement>> ExecuteRequestAsync(ServiceName serviceName, XDocument requestDoc);

        /// <summary>
        /// Execute a request.
        /// </summary>
        /// <param name="serviceName">Name of the SOAP service</param>
        /// <param name="requestBody">SOAP content as string</param>
        /// <returns>Response status and content</returns>
        Task<Response<XElement>> ExecuteRequestAsync(ServiceName serviceName, string requestBody);
    }
}
