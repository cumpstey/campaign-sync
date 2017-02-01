using System;
using System.Text.RegularExpressions;

namespace Zone.Campaign
{
    /// <summary>
    /// Class representing Campaign's internal name identifier.
    /// </summary>
    [Serializable]
    public sealed class InternalName
    {
        #region Fields

        private static readonly Regex ParseRegex = new Regex(@"^((?<namespace>[a-z]{3}):)?(?<name>[a-z0-9_\.]*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="InternalName"/>
        /// </summary>
        public InternalName()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InternalName"/>
        /// </summary>
        /// <param name="namespace">Namespace - this can be null</param>
        /// <param name="name">Name</param>
        public InternalName(string @namespace, string name)
        {
            Namespace = @namespace;
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Namespace.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether a namespace is specified.
        /// </summary>
        public bool HasNamespace
        {
            get { return !string.IsNullOrEmpty(Namespace); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parse an internal name string containing an optional namespace part and a name part.
        /// </summary>
        /// <param name="input">Interal name string</param>
        /// <returns>Parsed internal name</returns>
        public static InternalName Parse(string input)
        {
            var match = ParseRegex.Match(input);
            return !match.Success 
                ? null 
                : new InternalName(match.Groups["namespace"].Value, match.Groups["name"].Value);
        }

        /// <summary>
        /// Parse an internal name string containing an optional namespace part and a name part.
        /// </summary>
        /// <param name="input">Internal name string</param>
        /// <param name="result">Parsed internal name</param>
        /// <returns>Whether the parse succeeded</returns>
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

        /// <summary>
        /// Generates a formatted internal name string.
        /// </summary>
        /// <returns>Internal name string</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Namespace)
                       ? Name
                       : string.Format("{0}:{1}", Namespace, Name);
        }

        #endregion
    }
}
