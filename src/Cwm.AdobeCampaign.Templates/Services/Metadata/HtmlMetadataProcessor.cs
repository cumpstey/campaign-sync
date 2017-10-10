using System;
using System.IO;
using System.Text.RegularExpressions;
using Cwm.AdobeCampaign.Templates.Model;
using HtmlAgilityPack;

namespace Cwm.AdobeCampaign.Templates.Services.Metadata
{
    /// <summary>
    /// Provides functions for processing HTML files.
    /// </summary>
    public class HtmlMetadataProcessor : IMetadataExtractor, IMetadataInserter
    {
        #region Fields

        private static readonly Regex MetadataCommentRegex = new Regex("^<!--!(?<value>.*)!-->$", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly string MetadataFormat = string.Format("<!--!{0}{{0}}{0}!-->{{1}}", Environment.NewLine);

        private readonly IMetadataParser _metadataParser;

        private readonly IMetadataFormatter _metadataFormatter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="HtmlMetadataProcessor"/>
        /// </summary>
        /// <param name="metadataParser">Metadata parser</param>
        /// <param name="metadataFormatter">Metadata formatter</param>
        public HtmlMetadataProcessor(IMetadataParser metadataParser, IMetadataFormatter metadataFormatter)
        {
            _metadataParser = metadataParser;
            _metadataFormatter = metadataFormatter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Extract the code and metadata from raw HTML file content.
        /// </summary>
        /// <param name="input">Raw HTML file content</param>
        /// <returns>Class containing code content and metadata.</returns>
        public Template ExtractMetadata(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            // Parse html.
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(input);

            // Read metadata from first correctly formatted comment.
            var metadata = default(TemplateMetadata);
            var commentNodes = htmlDocument.DocumentNode.SelectNodes("//comment()");
            if (commentNodes != null)
            {
                // Remove all metadata comments, and use the value from the first one in the file.
                foreach (var commentNode in commentNodes)
                {
                    var match = MetadataCommentRegex.Match(commentNode.InnerText);
                    if (!match.Success)
                    {
                        continue;
                    }

                    commentNode.Remove();
                    metadata = _metadataParser.Parse(match.Groups["value"].Value);
                    break;
                }
            }

            // Generate output as string.
            var outputWriter = new StringWriter();
            htmlDocument.Save(outputWriter);
            var output = outputWriter.ToString().TrimStart();

            return new Template
            {
                Code = output,
                Metadata = metadata,
            };
        }

        /// <summary>
        /// Converts metadata and code into raw HTML file content which can be written to disk.
        /// </summary>
        /// <param name="input">Metadata and code</param>
        /// <returns>Raw HTML file content</returns>
        public string InsertMetadata(Template input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var formattedMetadata = _metadataFormatter.Format(input.Metadata);
            var code = string.Format(MetadataFormat, formattedMetadata, input.Code);
            return code;
        }

        #endregion
    }
}
