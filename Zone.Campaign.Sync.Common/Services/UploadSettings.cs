using System;
using System.Collections.Generic;

namespace Zone.Campaign.Sync.Services
{
    public class UploadSettings
    {
        public IList<string> FilePaths { get; set; }

        public IList<Tuple<string, string>> Replacements { get; set; }

        public bool TestMode { get; set; }
    }
}
