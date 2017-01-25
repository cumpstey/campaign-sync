using System;
using System.Collections.Generic;
using System.Linq;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Abstract;
using Zone.Campaign.WebServices.Services.Responses;
using log4net;

namespace Zone.Campaign.WebServices.Services
{
    public class ImagePersistService : Service, IImageWriteService
    {
        #region Fields

        private const string ServiceNamespace = "zon:persist";

        private static readonly ILog Log = LogManager.GetLogger(typeof(ImagePersistService));

        private readonly ISoapRequestHandler _requestHandler;

        #endregion

        #region Constructor

        public ImagePersistService(ISoapRequestHandler requestHandler)
        {
            if (requestHandler == null)
            {
                throw new ArgumentNullException(nameof(requestHandler));
            }

            _requestHandler = requestHandler;
        }

        #endregion

        #region Methods

        public Response WriteImage(Uri uri, IEnumerable<string> customHeaders, Tokens tokens, ImageFile item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            const string serviceName = "WriteImage";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest(serviceName, serviceNs, tokens);
            var requestDoc = serviceElement.OwnerDocument;

            // Build request for this service.
            var domElement = serviceElement.AppendChild("urn:domDoc", serviceNs);

            var itemXml = item.GetXmlForPersist(requestDoc);
            domElement.AppendChild(itemXml);

            // Execute request and get response from server.
            var response = _requestHandler.ExecuteRequest(uri, customHeaders, tokens, serviceName, ServiceNamespace, requestDoc);
            Log.DebugFormat("Response to {0} received: {1}", serviceName, response.Status);
            if (!response.Success)
            {
                return new Response<string>(response.Status, response.Message, response.Exception);
            }

            // Parse response to extract data as string.
            var outputNode = SelectSingleNode(response.Data, "urn:output", serviceNs);

            return new Response<string>(ResponseStatus.Success, outputNode.InnerText);
        }

        #endregion
    }
}
