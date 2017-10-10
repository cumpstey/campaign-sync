using System;
using System.Collections.Generic;
using System.Text;

namespace Cwm.AdobeCampaign.Sync.UI
{
    public class Options
    {
        public string ServerUrl { get { return "http://nl.barratt.local:8080/nl/jsp/soaprouter.jsp"; } }

        public string[] CustomHeaders { get { return null; } }
    }
}
