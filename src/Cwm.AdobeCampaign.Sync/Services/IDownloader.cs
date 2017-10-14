using System.Threading.Tasks;
using Cwm.AdobeCampaign.WebServices.Services;

namespace Cwm.AdobeCampaign.Sync.Services
{
    /// <summary>
    /// Contains methods to download files from Campaign and save them to disk.
    /// </summary>
    public interface IDownloader
    {
        #region Methods

        /// <summary>
        /// Download files based on parameters defined in the settings.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="settings">Download settings</param>
        Task DoDownloadAsync(IRequestHandler requestHandler, DownloadSettings settings);

        #endregion
    }
}
