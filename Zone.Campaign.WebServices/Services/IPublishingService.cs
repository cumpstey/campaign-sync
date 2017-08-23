using System.Collections.Generic;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
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
        Response<IEnumerable<KeyValuePair<int, bool>>> PublishTriggeredMessageInstances(IRequestHandler requestHandler);

        #endregion
    }
}
