using System;
using Zone.Campaign.WebServices.Security;
using Zone.Progress;

namespace Zone.Campaign.Sync.Services
{
    public interface IUploader
    {
        #region Methods

        void DoUpload(Uri uri, Tokens tokens, UploadSettings settings, IProgress<double> progress);

        void DoImageUpload(Uri uri, Tokens tokens, UploadSettings settings, IProgress<double> progress);

        #endregion
    }
}
