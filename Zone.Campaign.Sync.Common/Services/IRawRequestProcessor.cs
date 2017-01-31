using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.Services
{
    /// <summary>
    /// Contains methods to process a raw SOAP request stored in a file on disk.
    /// </summary>
    public interface IRawRequestProcessor
    {
        #region Methods

        /// <summary>
        /// Process a raw SOAP request stored in a file on disk.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="filePath">Path to file containing raw request</param>
        void ProcessRequest(IRequestHandler requestHandler, string filePath);

        #endregion
    }
}
