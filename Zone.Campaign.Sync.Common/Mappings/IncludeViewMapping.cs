using System;
using System.Collections.Generic;
using System.Xml;
using Zone.Campaign.Templates;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;
using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.Mappings
{
    /// <summary>
    /// Contains helper methods for mapping between the <see cref="IncludeView"/> .NET class and information formatted for Campaign to understand.
    /// </summary>
    public class IncludeViewMapping : FolderItemMapping<IncludeView>
    {
        #region Fields

        private const string FormatSeparator = "<%---- text above, html below ----%>";

        private readonly string[] _queryFields = { "@name", "@label", "@visible", "source/@dependOnFormat", "source/text", "source/html", "folder/@name" };

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="IncludeViewMapping"/>
        /// </summary>
        /// <param name="queryService">Query service</param>
        public IncludeViewMapping(IQueryService queryService)
            : base(queryService)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// List of field names which should be requested when querying Campaign.
        /// </summary>
        public override IEnumerable<string> QueryFields { get { return _queryFields; } }

        #endregion

        #region Methods

        /// <summary>
        /// Map the information parsed from a file into a class which can be sent to Campaign to be saved.
        /// </summary>
        /// <param name="requestHandler">Request handler, which can be used if further information from Campaign is required for the mapping.</param>
        /// <param name="template">Class containing file content and metadata.</param>
        /// <returns>Class containing information which can be sent to Campaign</returns>
        public override IPersistable GetPersistableItem(IRequestHandler requestHandler, Template template)
        {
            var item = new IncludeView
            {
                Name = template.Metadata.Name,
                Label = template.Metadata.Label,
            };

            if (template.Metadata.AdditionalProperties.ContainsKey("Folder"))
            {
                item.FolderId = GetFolderId(requestHandler, template.Metadata.AdditionalProperties["Folder"]);
            }

            if (template.Code.Contains(FormatSeparator))
            {
                item.VariesByFormat = true;
                item.TextCode = template.Code.Substring(0, template.Code.IndexOf(FormatSeparator)).Trim();
                item.HtmlCode = template.Code.Substring(template.Code.LastIndexOf(FormatSeparator) + FormatSeparator.Length).Trim();
            }
            else
            {
                item.VariesByFormat = false;
                item.TextCode = template.Code;
            }

            bool visible;
            if (template.Metadata.AdditionalProperties.ContainsKey("Visible")
                && bool.TryParse(template.Metadata.AdditionalProperties["Visible"], out visible))
            {
                item.IncludeInCustomisationMenus = visible;
            }

            return item;
        }

        /// <summary>
        /// Map the information sent back by Campaign into a format which can be saved as a file to disk.
        /// </summary>
        /// <param name="requestHandler">Request handler, which can be used if further information from Campaign is required for the mapping.</param>
        /// <param name="rawQueryResponse">Raw response from Campaign.</param>
        /// <returns>Class containing file content and metadata</returns>
        public override Template ParseQueryResponse(IRequestHandler requestHandler, string rawQueryResponse)
        {
            var doc = new XmlDocument();
            doc.LoadXml(rawQueryResponse);

            var metadata = new TemplateMetadata
            {
                Schema = InternalName.Parse(IncludeView.Schema),
                Name = new InternalName(null, doc.DocumentElement.Attributes["name"].InnerText),
                Label = doc.DocumentElement.Attributes["label"].InnerText,
            };

            var folderInternalName = doc.DocumentElement.SelectSingleNode("folder").Attributes["name"].InnerText;
            metadata.AdditionalProperties.Add("Folder", folderInternalName);

            var visibleAttribute = doc.DocumentElement.Attributes["visible"];
            if (visibleAttribute != null)
            {
                metadata.AdditionalProperties.Add("Visible", (visibleAttribute.InnerText == "1").ToString());
            }

            var textCodeNode = doc.DocumentElement.SelectSingleNode("source/text");
            var rawTextCode = textCodeNode == null
                          ? string.Empty
                          : textCodeNode.InnerText;

            var htmlCodeNode = doc.DocumentElement.SelectSingleNode("source/html");
            var rawHtmlCode = htmlCodeNode == null
                          ? string.Empty
                          : htmlCodeNode.InnerText;

            var dependOnFormat = false;
            var dependOnFormatAttribute = doc.DocumentElement.SelectSingleNode("source/@dependOnFormat");
            if (dependOnFormatAttribute != null)
            {
                dependOnFormat = dependOnFormatAttribute.InnerText == "true";
            }

            var rawCode = dependOnFormat
                ? string.Concat(rawTextCode, Environment.NewLine, Environment.NewLine, FormatSeparator, Environment.NewLine, Environment.NewLine, rawHtmlCode)
                : rawTextCode;

            return new Template
            {
                Code = rawCode,
                Metadata = metadata,
                FileExtension = FileTypes.Jssp,
            };
        }

        #endregion
    }
}
