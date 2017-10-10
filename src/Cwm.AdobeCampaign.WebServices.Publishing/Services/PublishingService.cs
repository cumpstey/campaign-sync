using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Cwm.AdobeCampaign.WebServices.Services.Abstract;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using Microsoft.Extensions.Logging;

namespace Cwm.AdobeCampaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the zon:publishing SOAP services.
    /// </summary>
    public class PublishingService : Service, IPublishingService
    {
        #region Fields

        public const string ServiceNamespace = "cwm:publishing";

        public const string PublishTriggeredMessageInstancesServiceName = "PublishTriggeredMessageInstances";

        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public PublishingService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PublishingService>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Publish all triggered message instances.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <returns>Response, with the returned id of the event</returns>
        public async Task<Response<IEnumerable<KeyValuePair<int, bool>>>> PublishTriggeredMessageInstancesAsync(IRequestHandler requestHandler)
        {
            XNamespace serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var requestDoc = CreateServiceRequest(PublishTriggeredMessageInstancesServiceName, serviceNs);

            // Execute request and get response from server.
            var response = await requestHandler.ExecuteRequestAsync(new ServiceName(ServiceNamespace, PublishTriggeredMessageInstancesServiceName), requestDoc);

            _logger.LogDebug($"Response to {PublishTriggeredMessageInstancesServiceName} received: {response.Status}");

            if (!response.Success)
            {
                return new Response<IEnumerable<KeyValuePair<int, bool>>>(response.Status, response.Message, response.Exception);
            }

            // Parse response to extract returned data.
            // This should always be there - any unsuccessful response should be caught above.
            try
            {
                var returnedData = new List<KeyValuePair<int, bool>>();
                var deliveryElements = response.Data.Element(serviceNs + "deliveries").Elements(serviceNs + "delivery");
                foreach (var deliveryElement in deliveryElements)
                {
                    var id = int.Parse(deliveryElement.Attribute("id").Value);
                    var success = bool.Parse(deliveryElement.Attribute("success").Value);
                    returnedData.Add(new KeyValuePair<int, bool>(id, success));
                }

                return new Response<IEnumerable<KeyValuePair<int, bool>>>(ResponseStatus.Success, returnedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error parsing {PublishTriggeredMessageInstancesServiceName} response.");
                return new Response<IEnumerable<KeyValuePair<int, bool>>>(ResponseStatus.ParsingError, $"Failed to parse {PublishTriggeredMessageInstancesServiceName} response", ex);
            }
        }

        #endregion
    }
}
