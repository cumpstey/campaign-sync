using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;
using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services
{
    public class TransformParameters
    {
        public string OriginalFileName { get; set; }

        public bool ApplyTransforms { get; set; }
    }
}
