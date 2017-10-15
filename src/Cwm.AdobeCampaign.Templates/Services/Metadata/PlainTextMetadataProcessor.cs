using System;
using System.Text.RegularExpressions;
using Cwm.AdobeCampaign.Templates.Model;

namespace Cwm.AdobeCampaign.Templates.Services.Metadata
{
    /// <summary>
    /// Provides functions for processing plain text files.
    /// </summary>
    public class PlainTextMetadataProcessor : IMetadataExtractor, IMetadataInserter
    {
        #region Fields

        private static readonly Regex MetadataRegex = new Regex(@"^(?<value>.*?)!----!\s*", RegexOptions.Singleline);

        private static readonly string MetadataFormat = string.Format("{{0}}{0}!----!{0}{{1}}", Environment.NewLine);

        private readonly IMetadataParser _metadataParser;

        private readonly IMetadataFormatter _metadataFormatter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="PlainTextMetadataProcessor"/>
        /// </summary>
        /// <param name="metadataParser">Metadata parser</param>
        /// <param name="metadataFormatter">Metadata formatter</param>
        public PlainTextMetadataProcessor(IMetadataParser metadataParser, IMetadataFormatter metadataFormatter)
        {
            _metadataParser = metadataParser;
            _metadataFormatter = metadataFormatter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Extract the code and metadata from raw JavaScript Server Pages file content.
        /// </summary>
        /// <param name="input">Raw JavaScript Server Pages file content</param>
        /// <returns>Class containing code content and metadata.</returns>
        public Template ExtractMetadata(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            // Read metadata from correctly formatted comment.
            var match = MetadataRegex.Match(input);
            var output = input;
            var metadata = new TemplateMetadata();
            if (match.Success)
            {
                var fullCapture = match.Groups[0];
                output = output.Remove(fullCapture.Index, fullCapture.Length);
                metadata = _metadataParser.Parse(match.Groups["value"].Value);
            }

            return new Template
            {
                Code = output,
                Metadata = metadata,
            };
        }

        /// <summary>
        /// Converts metadata and code into raw JavaScript Server Pages file content which can be written to disk.
        /// </summary>
        /// <param name="input">Metadata and code</param>
        /// <returns>Raw JavaScript Server Pages file content</returns>
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
