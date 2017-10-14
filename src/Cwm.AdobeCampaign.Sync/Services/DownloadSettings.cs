using System.Collections.Generic;

namespace Cwm.AdobeCampaign.Sync.Services
{
    /// <summary>
    /// Settings used in downloading files.
    /// </summary>
    public class DownloadSettings
    {
        /// <summary>
        /// Local directory in which to create the files.
        /// </summary>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Whether to create all files in the same directory,
        /// or create subdirectories based on underscore delimiting.
        /// </summary>
        public SubdirectoryMode SubdirectoryMode { get; set; }

        /// <summary>
        /// Schema of the items to download.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Query conditions.
        /// </summary>
        public string[] Conditions { get; set; }
    }
}
