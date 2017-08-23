using System.Collections.Generic;
using log4net;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Services.Abstract;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the nms:rtEvent SOAP services.
    /// </summary>
    public class TriggeredMessageService : Service, ITriggeredMessageService
    {
        #region Fields

        private const string ServiceNamespace = "nms:rtEvent";

        private static readonly ILog Log = LogManager.GetLogger(typeof(TriggeredMessageService));

        #endregion

        #region Methods

        /// <summary>
        /// Push a real time event in order to trigger an email.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="eventDetails">The event to push; this must be a <see cref="Event"/></param>
        /// <returns>Response, with the returned id of the event</returns>
        public Response<int> PushRealTimeEvent(IRequestHandler requestHandler, Event eventDetails)
        {
            const string serviceName = "PushRtEvent";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest(serviceName, serviceNs);
            var requestDoc = serviceElement.OwnerDocument;

            // Build request for this service.
            var domElement = serviceElement.AppendChild("urn:domDoc", serviceNs);

            var eventElement = domElement.AppendChild("rtEvent");
            eventElement.AppendAttribute("type", eventDetails.EventType);
            eventElement.AppendAttribute("email", eventDetails.Email);
            eventElement.AppendAttribute("origin", eventDetails.Origin);
            eventElement.AppendAttribute("wishedChannel", eventDetails.WishedChannel.ToString().ToLower());
            eventElement.AppendAttribute("externalId", eventDetails.ExternalId);

            if (eventDetails.ContextData != null && eventDetails.ContextData.DocumentElement != null && eventDetails.ContextData.DocumentElement.Name == "ctx")
            {
                var ctxElement = requestDoc.ImportNode(eventDetails.ContextData.DocumentElement, true);
                eventElement.AppendChild(ctxElement);
            }

            // Execute request and get response from server.
            var response = requestHandler.ExecuteRequest(new ServiceName(ServiceNamespace, serviceName), requestDoc);
            if (!response.Success)
            {
                return new Response<int>(response.Status, response.Message, response.Exception);
            }

            Log.Debug($"Response to {serviceName} received: {response.Status}");

            // Parse response to extract returned event id.
            var returnedId = 0;
            if (response.Data != null)
            {
                var idElement = SelectSingleNode(response.Data, "urn:plId", serviceNs);
                if (idElement != null)
                {
                    int.TryParse(idElement.InnerText, out returnedId);
                }
            }

            return new Response<int>(ResponseStatus.Success, returnedId);
        }

        #endregion
    }
}
