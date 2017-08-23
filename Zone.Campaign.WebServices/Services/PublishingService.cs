using System.Collections.Generic;
using System.Linq;
using System.Xml;
using log4net;
using Zone.Campaign.WebServices.Services.Abstract;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the zon:publishing SOAP services.
    /// </summary>
    public class PublishingService : Service, IPublishingService
    {
        #region Fields

        private const string ServiceNamespace = "zon:publishing";

        private static readonly ILog Log = LogManager.GetLogger(typeof(PublishingService));

        #endregion

        #region Methods

        /// <summary>
        /// Publish all triggered message instances.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <returns>Response, with the returned id of the event</returns>
        public Response<IEnumerable<KeyValuePair<int, bool>>> PublishTriggeredMessageInstances(IRequestHandler requestHandler)
        {
            const string serviceName = "PublishTriggeredMessageInstances";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest(serviceName, serviceNs);
            var requestDoc = serviceElement.OwnerDocument;

            // Execute request and get response from server.
            var response = requestHandler.ExecuteRequest(new ServiceName(ServiceNamespace, serviceName), requestDoc);
            if (!response.Success)
            {
                return new Response<IEnumerable<KeyValuePair<int, bool>>>(response.Status, response.Message, response.Exception);
            }

            Log.Debug($"Response to {serviceName} received: {response.Status}");

            // Parse response to extract returned event id.
            var returnedData = new List<KeyValuePair<int, bool>>();
            if (response.Data != null)
            {
                var deliveryElements = SelectNodes(response.Data, "urn:deliveries/urn:delivery", serviceNs);
                if (deliveryElements != null)
                {
                    foreach (var deliveryElement in deliveryElements.Cast<XmlElement>())
                    {
                        var idAttribute = deliveryElement.Attributes["id"];
                        var successAttribute = deliveryElement.Attributes["success"];
                        int id;
                        bool success;
                        if (idAttribute != null && int.TryParse(idAttribute.InnerText, out id)
                            && successAttribute != null && bool.TryParse(successAttribute.InnerText, out success))
                        {
                            returnedData.Add(new KeyValuePair<int, bool>(id, success));
                        }
                    }
                }
            }

            return new Response<IEnumerable<KeyValuePair<int, bool>>>(ResponseStatus.Success, returnedData);
        }

        #endregion
    }
}
