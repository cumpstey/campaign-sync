using Cwm.AdobeCampaign.WebServices.Model;
using Cwm.AdobeCampaign.WebServices.Services.Responses;

namespace Cwm.AdobeCampaign.WebServices.Services
{
    /// <summary>
    /// Contains image upload functions.
    /// </summary>
    public interface IImageWriteService
    {
        #region Methods

        /// <summary>
        /// Upload an image and create/update an xtk:fileRes record.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="item">Image file and metadata</param>
        /// <returns>Response</returns>
        Response WriteImage(IRequestHandler requestHandler, ImageResource item);
        
        #endregion
    }
}
