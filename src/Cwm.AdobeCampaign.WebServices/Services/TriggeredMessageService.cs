using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cwm.AdobeCampaign.WebServices.Model;
using Cwm.AdobeCampaign.WebServices.Services.Abstract;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
#if NETSTANDARD2_0
using Microsoft.Extensions.Logging;
#endif

namespace Cwm.AdobeCampaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the nms:rtEvent SOAP services.
    /// </summary>
    public class TriggeredMessageService : Service, ITriggeredMessageService
    {
        #region Fields

        /// <summary>
        /// Soap namespace of the real time event services.
        /// </summary>
        public const string RealTimeEventServiceNamespace = "nms:rtEvent";

        /// <summary>
        /// Soap name of the push real time event service.
        /// </summary>
        public const string PushRealTimeEventServiceName = "PushEvent";

#if NETSTANDARD2_0
        private readonly ILogger _logger;
#endif

        #endregion

        #region Constructor

#if NETSTANDARD2_0
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggeredMessageService"/> class. 
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        public TriggeredMessageService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TriggeredMessageService>();
        }
#endif

        #endregion

        #region Methods

        /// <summary>
        /// Push a real time event in order to trigger an email.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="eventDetails">The event to push; this must be a <see cref="Event"/></param>
        /// <returns>Response, with the returned id of the event</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="requestHandler"/> is null.</exception>
        public async Task<Response<long?>> PushRealTimeEventAsync(IRequestHandler requestHandler, Event eventDetails)
        {
            if (requestHandler == null) throw new ArgumentNullException(nameof(requestHandler));

            XNamespace serviceNs = string.Concat("urn:", RealTimeEventServiceNamespace);

            // Create common elements of SOAP request.
            var requestDoc = CreateServiceRequest(PushRealTimeEventServiceName, serviceNs);
            var serviceElement = GetServiceElement(requestDoc, PushRealTimeEventServiceName, serviceNs);

            // Build request for this service.
            serviceElement.Add(new XElement(serviceNs + "domDoc",
                                   new XElement("rtEvent",
                                       new XAttribute("type", eventDetails.EventType ?? string.Empty),
                                       new XAttribute("email", eventDetails.Email ?? string.Empty),
                                       new XAttribute("origin", eventDetails.Origin ?? string.Empty),
                                       new XAttribute("wishedChannel", eventDetails.WishedChannel.ToString().ToLower()),
                                       new XAttribute("externalId", eventDetails.ExternalId ?? string.Empty),
                                       eventDetails.ContextData ?? new XElement("ctx"))));

            // Execute request and get response from server.
            var response = await requestHandler.ExecuteRequestAsync(new ServiceName(RealTimeEventServiceNamespace, PushRealTimeEventServiceName), requestDoc);
            if (!response.Success)
            {
                return new Response<long?>(response.Status, response.Message, response.Exception);
            }

#if NETSTANDARD2_0
            _logger.LogDebug($"Response to {PushRealTimeEventServiceName} received: {response.Status}");
#endif

            // Parse response to extract returned event id.
            // This should always be there - any unsuccessful response should be caught above.
            try
            {
                var id = long.Parse(response.Data.Element(serviceNs + "plId").Value);
                return new Response<long?>(ResponseStatus.Success, id);
            }
            catch (Exception ex)
            {
#if NETSTANDARD2_0
                _logger.LogError(ex, $"Error parsing {PushRealTimeEventServiceName} response.");
#endif
                return new Response<long?>(ResponseStatus.ParsingError, $"Failed to parse {PushRealTimeEventServiceName} response", ex);
            }
        }

        #endregion
    }
}
