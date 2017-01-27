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
    /// <summary>
    /// Wrapper for the zon:persist SOAP services.
    /// </summary>
    public class ImagePersistService : Service, IImageWriteService
    {
        #region Fields

        private const string ServiceNamespace = "zon:persist";

        private static readonly ILog Log = LogManager.GetLogger(typeof(ImagePersistService));

        private readonly ISoapRequestHandler _requestHandler;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="ImagePersistService"/>
        /// </summary>
        /// <param name="requestHandler">Handler for the SOAP requests</param>
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

        /// <summary>
        /// Upload an image and create/update an xtk:fileRes record.
        /// </summary>
        /// <param name="tokens">Authenication tokens</param>
        /// <param name="item">Image file and metadata</param>
        /// <returns>Response</returns>
        public Response WriteImage(Tokens tokens, ImageFile item)
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
            var response = _requestHandler.ExecuteRequest(tokens, ServiceNamespace, serviceName, requestDoc);
            Log.Debug($"Response to {serviceName} received: {response.Status}");
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
