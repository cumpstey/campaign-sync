using System.Threading.Tasks;
using Cwm.AdobeCampaign.WebServices.Model;
using Cwm.AdobeCampaign.WebServices.Services.Responses;

namespace Cwm.AdobeCampaign.WebServices.Services
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
        Task<Response<long?>> PushRealTimeEventAsync(IRequestHandler requestHandler, Event eventDetails);
        
        #endregion
    }
}
