using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.Services
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
        void DoDownload(IRequestHandler requestHandler, DownloadSettings settings);

        #endregion
    }
}
