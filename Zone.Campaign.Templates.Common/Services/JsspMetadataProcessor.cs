using System;
using System.Linq;
using System.Text.RegularExpressions;
using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services
{
    public class JsspMetadataProcessor : IMetadataExtractor, IMetadataInserter
    {
        #region Fields

        private static readonly Regex MetadataRegex = new Regex(@"\\<%--!(?<value>.*?)!--%\>\s*", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly string MetadataFormat = string.Format("<%--!{0}{{0}}{0}!--%>{0}{{1}}", Environment.NewLine);

        private readonly IMetadataParser _metadataParser;

        private readonly IMetadataFormatter _metadataFormatter;

        #endregion

        #region Constructor

        public JsspMetadataProcessor()
        {
            _metadataParser = new MetadataParser();
            _metadataFormatter = new MetadataParser();
        }

        #endregion

        #region Methods

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
