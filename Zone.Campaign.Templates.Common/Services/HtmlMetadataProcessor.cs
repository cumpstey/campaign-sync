using System;
using System.IO;
using System.Text.RegularExpressions;
using Zone.Campaign.Templates.Model;
using HtmlAgilityPack;

namespace Zone.Campaign.Templates.Services
{
    public class HtmlMetadataProcessor : IMetadataExtractor, IMetadataInserter
    {
        #region Fields

        private static readonly Regex MetadataCommentRegex = new Regex("^<!--!(?<value>.*)!-->$", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly string MetadataFormat = string.Format("<!--!{0}{{0}}{0}!-->{{1}}", Environment.NewLine);

        private readonly IMetadataParser _metadataParser;

        private readonly IMetadataFormatter _metadataFormatter;

        #endregion

        #region Constructor

        public HtmlMetadataProcessor()
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
           var output = outputWriter.ToString();

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

            var formattedMetadata = _metadataFormatter.Format(input.Metadata);
            var code = string.Format(MetadataFormat, formattedMetadata, input.Code);
            return code;
        }

        #endregion
    }
}
