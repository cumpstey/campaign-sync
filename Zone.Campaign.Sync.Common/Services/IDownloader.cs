using System;
using Zone.Campaign.WebServices.Security;

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
        /// <param name="tokens">Authentication tokens</param>
        /// <param name="settings">Download settings</param>
        void DoDownload(Tokens tokens, DownloadSettings settings);

        #endregion
    }
}
