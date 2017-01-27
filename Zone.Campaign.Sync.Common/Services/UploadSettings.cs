using System;
using System.Collections.Generic;

namespace Zone.Campaign.Sync.Services
{
    /// <summary>
    /// Settings used in uploading files.
    /// </summary>
    public class UploadSettings
    {
        /// <summary>
        /// Collection of paths of the files to upload.
        /// These can be single files, directories (recursive) or wilcard matches.
        /// </summary>
        public IList<string> FilePaths { get; set; }

        /// <summary>
        /// Replacements to make in the file contents prior to upload.
        /// </summary>
        public IList<Tuple<string, string>> Replacements { get; set; }

        /// <summary>
        /// Test mode: Report what would have been uploaded, but don't actually upload any files.
        /// </summary>
        public bool TestMode { get; set; }
    }
}
