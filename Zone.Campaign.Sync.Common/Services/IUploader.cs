using Zone.Campaign.WebServices.Services;

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
        /// <param name="requestHandler">Request handler</param>
        /// <param name="settings">Upload settings</param>
        void DoUpload(IRequestHandler requestHandler, UploadSettings settings);

        /// <summary>
        /// Upload a set of images defined by the settings.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="settings">Upload settings</param>
        void DoImageUpload(IRequestHandler requestHandler, UploadSettings settings);

        #endregion
    }
}
