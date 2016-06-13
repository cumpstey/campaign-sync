using System;
using System.Text.RegularExpressions;

namespace Zone.Campaign
{
    [Serializable]
    public class InternalName
    {
        #region Fields

        private static readonly Regex ParseRegex = new Regex(@"^((?<namespace>[a-z]{3}):)?(?<name>[a-z0-9_\.]*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Constructor

        public InternalName()
        {
        }

        public InternalName(string @namespace, string name)
        {
            Namespace = @namespace;
            Name = name;
        }

        #endregion

        #region Properties

        public string Namespace { get; set; }

        public string Name { get; set; }

        public bool HasNamespace
        {
            get { return !string.IsNullOrEmpty(Namespace); }
        }

        #endregion

        #region Methods

        public static InternalName Parse(string input)
        {
            var match = ParseRegex.Match(input);
            return !match.Success 
                ? null 
                : new InternalName(match.Groups["namespace"].Value, match.Groups["name"].Value);
        }

        public static bool TryParse(string input, out InternalName result)
        {
            var match = ParseRegex.Match(input);
            if (!match.Success)
            {
                result = null;
                return false;
            }

            result = new InternalName(match.Groups["namespace"].Value, match.Groups["name"].Value);
            return true;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Namespace)
                       ? Name
                       : string.Format("{0}:{1}", Namespace, Name);
        }

        #endregion
    }
}
