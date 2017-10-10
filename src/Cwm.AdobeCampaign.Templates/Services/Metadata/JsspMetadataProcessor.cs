using System;
using System.Text.RegularExpressions;
using Cwm.AdobeCampaign.Templates.Exceptions;
using Cwm.AdobeCampaign.Templates.Model;

namespace Cwm.AdobeCampaign.Templates.Services.Metadata
{
    /// <summary>
    /// Provides functions for processing JavaScript Server Pages files.
    /// </summary>
    public class JsspMetadataProcessor : IMetadataExtractor, IMetadataInserter
    {
        #region Fields

        private static readonly Regex MetadataRegex = new Regex(@"\<%--!(?<value>.*?)!--%\>\s*", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly string MetadataFormat = string.Format("<%--!{0}{{0}}{0}!--%>{0}{{1}}", Environment.NewLine);

        private readonly IMetadataParser _metadataParser;

        private readonly IMetadataFormatter _metadataFormatter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="JsspMetadataProcessor"/>
        /// </summary>
        /// <param name="metadataParser">Metadata parser</param>
        /// <param name="metadataFormatter">Metadata formatter</param>
        public JsspMetadataProcessor(IMetadataParser metadataParser, IMetadataFormatter metadataFormatter)
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
        /// <exception cref="MultipleMetadataException">Thrown when more than one metadata block is found in the content</exception>
        public Template ExtractMetadata(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            // Read metadata from first correctly formatted comment.
            var match = MetadataRegex.Matches(input);
            var output = input;
            var metadata = new TemplateMetadata();
            switch (match.Count)
            {
                case 0:
                    break;
                case 1:
                    var fullCapture = match[0].Groups[0];
                    output = output.Remove(fullCapture.Index, fullCapture.Length);
                    metadata = _metadataParser.Parse(match[0].Groups["value"].Value);
                    break;
                default:
                    throw new MultipleMetadataException(match.Count);
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
