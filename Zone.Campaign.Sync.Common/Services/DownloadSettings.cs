using System.Collections.Generic;

namespace Zone.Campaign.Sync.Services
{
    public class DownloadSettings
    {
        public string OutputDirectory { get; set; }

        public SubdirectoryMode SubdirectoryMode { get; set; }

        public string Schema { get; set; }

        public string[] Conditions { get; set; }
    }
}
