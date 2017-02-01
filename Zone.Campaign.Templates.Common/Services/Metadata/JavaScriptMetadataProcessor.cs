using System;
using System.Text.RegularExpressions;
using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services.Metadata
{
    /// <summary>
    /// Provides functions for processing JavaScript files.
    /// </summary>
    public class JavaScriptMetadataProcessor : IMetadataExtractor, IMetadataInserter
    {
        #region Fields

        private static readonly Regex MetadataRegex = new Regex(@"\/\*!(?<value>.*?)!\*\/\s*", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly string MetadataFormat = string.Format("/*!{0}{{0}}{0}!*/{0}{{1}}", Environment.NewLine);

        private readonly IMetadataParser _metadataParser;

        private readonly IMetadataFormatter _metadataFormatter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="JavaScriptMetadataProcessor"/>
        /// </summary>
        public JavaScriptMetadataProcessor()
        {
            _metadataParser = new MetadataProcessor();
            _metadataFormatter = new MetadataProcessor();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Extract the code and metadata from raw JavaScript file content.
        /// </summary>
        /// <param name="input">Raw JavaScript file content</param>
        /// <returns>Class containing code content and metadata.</returns>
        public Template ExtractMetadata(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            // Read metadata from first correctly formatted comment.
            var match = MetadataRegex.Match(input);
            var output = input;
            var metadata = new TemplateMetadata();
            if (match.Success)
            {
                for (var i = match.Groups["value"].Captures.Count - 1; i >= 0; i--)
                {
                    var fullCapture = match.Groups[0].Captures[i];
                    output = output.Remove(fullCapture.Index, fullCapture.Length);
                    metadata = _metadataParser.Parse(match.Groups["value"].Captures[i].Value);
                }
            }

            return new Template
            {
                Code = output,
                Metadata = metadata,
            };
        }

        /// <summary>
        /// Converts metadata and code into raw JavaScript file content which can be written to disk.
        /// </summary>
        /// <param name="input">Metadata and code</param>
        /// <returns>Raw JavaScript file content</returns>
        public string InsertMetadata(Template input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            // Add comment to start of document.
            var formattedMetadata = _metadataFormatter.Format(input.Metadata);
            var code = string.Format(MetadataFormat, formattedMetadata, input.Code);
            return code;
        }

        #endregion
    }
}
