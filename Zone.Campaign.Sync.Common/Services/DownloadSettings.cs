using System.Collections.Generic;

namespace Zone.Campaign.Sync.Services
{
    public class DownloadSettings
    {
        public string OutputDirectory { get; set; }

        public string DirectoryMode { get; set; }

        public string Schema { get; set; }

        public IList<string> Conditions { get; set; }
    }
}
