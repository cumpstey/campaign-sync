using Zone.Campaign.WebServices.Security;

namespace Zone.Campaign.Sync.Services
{
    /// <summary>
    /// Contains methods to read files from disk and upload them into Campaign.
    /// </summary>
    public interface IUploader
    {
        #region Methods

        /// <summary>
        /// Upload a set of files defined by the settings.
        /// </summary>
        /// <param name="tokens">Authentication tokens</param>
        /// <param name="settings">Upload settings</param>
        void DoUpload(Tokens tokens, UploadSettings settings);

        /// <summary>
        /// Upload a set of images defined by the settings.
        /// </summary>
        /// <param name="tokens">Authentication tokens</param>
        /// <param name="settings">Upload settings</param>
        void DoImageUpload(Tokens tokens, UploadSettings settings);

        #endregion
    }
}
