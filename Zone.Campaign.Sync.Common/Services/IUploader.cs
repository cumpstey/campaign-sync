using System;
using Zone.Campaign.WebServices.Security;

namespace Zone.Campaign.Sync.Services
{
    public interface IUploader
    {
        #region Methods

        void DoUpload(Uri rootUri, Tokens tokens, UploadSettings settings);

        void DoImageUpload(Uri rootUri, Tokens tokens, UploadSettings settings);

        #endregion
    }
}
