using System;
using Zone.Campaign.WebServices.Security;

namespace Zone.Campaign.Sync.Services
{
    public interface IDownloader
    {
        #region Methods

        void DoDownload(Uri rootUri, Tokens tokens, DownloadSettings settings);

        #endregion
    }
}
