using System;
using System.Collections.Generic;
using System.Linq;
using Cwm.AdobeCampaign.WebServices.Model;
using Cwm.AdobeCampaign.WebServices.Model.Abstract;
using Cwm.AdobeCampaign.WebServices.Security;
using Cwm.AdobeCampaign.WebServices.Services.Abstract;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using log4net;

namespace Cwm.AdobeCampaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the zon:persist SOAP services.
    /// </summary>
    public class ImagePersistService : Service, IImageWriteService
    {
        #region Fields

        private const string ServiceNamespace = "zon:persist";

        private static readonly ILog Log = LogManager.GetLogger(typeof(ImagePersistService));

        #endregion

        #region Methods

        /// <summary>
        /// Upload an image and create/update an xtk:fileRes record.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="item">Image file and metadata</param>
        /// <returns>Response</returns>
        public Response WriteImage(IRequestHandler requestHandler, ImageResource item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            const string serviceName = "WriteImage";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest(serviceName, serviceNs);
            var requestDoc = serviceElement.OwnerDocument;

            // Build request for this service.
            var domElement = serviceElement.AppendChild("urn:domDoc", serviceNs);

            var itemXml = item.GetXmlForPersist(requestDoc);
            domElement.AppendChild(itemXml);

            // Execute request and get response from server.
            var response = requestHandler.ExecuteRequest(new ServiceName(ServiceNamespace, serviceName), requestDoc);
            Log.Debug($"Response to {serviceName} received: {response.Status}");
            if (!response.Success)
            {
                return new Response<string>(response.Status, response.Message, response.Exception);
            }

            return new Response<string>(ResponseStatus.Success);
        }

        #endregion
    }
}
