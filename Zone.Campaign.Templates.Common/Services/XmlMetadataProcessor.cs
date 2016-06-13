using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services
{
    public class XmlMetadataProcessor : IMetadataExtractor, IMetadataInserter
    {
        #region Fields

        private static readonly Regex MetadataCommentRegex = new Regex("^!(?<value>.*)!$", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly string MetadataFormat = string.Format("!{0}{{0}}{0}!", Environment.NewLine);

        private readonly IMetadataParser _metadataParser;

        private readonly IMetadataFormatter _metadataFormatter;

        #endregion

        #region Constructor

        public XmlMetadataProcessor()
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

            // Parse xml.
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(input);

            // Read metadata from first correctly formatted comment.
            var metadata = default(TemplateMetadata);
            var commentNodes = xmlDocument.DocumentElement.SelectNodes("//comment()");
            if (commentNodes != null)
            {
                // Remove all metadata comments, and use the value from the first one in the file.
                foreach (var commentNode in commentNodes.Cast<XmlComment>().Reverse())
                {
                    var match = MetadataCommentRegex.Match(commentNode.InnerText);
                    if (!match.Success)
                    {
                        continue;
                    }

                    commentNode.ParentNode.RemoveChild(commentNode);
                    metadata = _metadataParser.Parse(match.Groups["value"].Value);
                    break;
                }
            }

            var output = GenerateDocumentXml(xmlDocument);
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

            // Parse xml.
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(input.Code);

            // Add comment to start of document.
            var metadataString = _metadataFormatter.Format(input.Metadata);
            var metadataCommentNode = xmlDocument.CreateComment(string.Format(MetadataFormat, metadataString));
            xmlDocument.PrependChild(metadataCommentNode);

            return GenerateDocumentXml(xmlDocument);
        }

        #endregion

        #region Helpers

        private static string GenerateDocumentXml(XmlDocument xmlDocument)
        {
            // Format nicely: indent, and omit xml declaration if it wasn't present in the original.
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = xmlDocument.FirstChild.NodeType != XmlNodeType.XmlDeclaration,
            };

            // Generate document as string.
            // All this complexity is to get it correctly UTF-8 encoded, as otherwise it ends up UTF-8 encoded but
            // declared as UTF-16. :-(
            using (var outputStream = new MemoryStream())
            using (var outputWriter = new StreamWriter(outputStream, Encoding.UTF8))
            using (var xmlWriter = XmlWriter.Create(outputWriter, settings))
            {
                xmlDocument.Save(xmlWriter);

                var outputReader = new StreamReader(outputStream);
                outputStream.Position = 0;
                var output = outputReader.ReadToEnd();
                return output;
            }
        }

        #endregion
    }
}
