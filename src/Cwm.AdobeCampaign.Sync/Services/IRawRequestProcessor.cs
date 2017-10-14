using System.Threading.Tasks;
using Cwm.AdobeCampaign.WebServices.Services;

namespace Cwm.AdobeCampaign.Sync.Services
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
        Task ProcessRequestAsync(IRequestHandler requestHandler, string filePath);

        #endregion
    }
}
