using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Cwm.AdobeCampaign.WebServices.Security;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using System.Xml.Linq;
using System.Threading.Tasks;
using Cwm.AdobeCampaign.WebServices.Services;

namespace Cwm.AdobeCampaign.WebServices.Tests.Dummy
{
    public class DummyRequestHandler : IRequestHandler
    {
        #region Fields

        private readonly Response<XElement> _response;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="DummyRequestHandler"/>
        /// </summary>
        public DummyRequestHandler(Response<XElement> response)
        {
            _response = response;
        }

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
            return await Task.Run(() => _response);
        }

        #endregion
    }
}
