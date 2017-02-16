using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the nms:rtEvent SOAP services.
    /// </summary>
    public interface ITriggeredMessageService
    {
        #region Methods

        /// <summary>
        /// Push a real time event in order to trigger a delivery.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="eventDetails">The event to push</param>
        /// <returns>Response, with the returned id of the event</returns>
        Response<int> PushRealTimeEvent(IRequestHandler requestHandler, Event eventDetails);

        #endregion
    }
}
