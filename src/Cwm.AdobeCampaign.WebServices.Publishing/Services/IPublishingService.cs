using System.Collections.Generic;
using System.Threading.Tasks;
using Cwm.AdobeCampaign.WebServices.Services.Responses;

namespace Cwm.AdobeCampaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the zon:publishing SOAP services.
    /// </summary>
    public interface IPublishingService
    {
        #region Methods

        /// <summary>
        /// Publish all triggered message instances.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <returns>Response, with the returned id of the event</returns>
        Task<Response<IEnumerable<KeyValuePair<int, bool>>>> PublishTriggeredMessageInstancesAsync(IRequestHandler requestHandler);

        #endregion
    }
}
