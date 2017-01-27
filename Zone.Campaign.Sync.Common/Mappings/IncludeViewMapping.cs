using System;
using System.Collections.Generic;
using System.Xml;
using Zone.Campaign.Templates;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.Sync.Mappings
{
    public class IncludeViewMapping : Mapping<IncludeView>
    {
        #region Fields

        private readonly string[] _queryFields = { "@name", "@label", "@visible", "source/text", "source/html" };

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
        /// <param name="template">Class containing file content and metadata</param>
        /// <returns>Class containing information which can be sent to Campaign</returns>
        public override IPersistable GetPersistableItem(Template template)
        {
            var item = new IncludeView
            {
                Name = template.Metadata.Name,
                Label = template.Metadata.Label,
                TextCode = template.Code,
            };

            bool visible;
            if (template.Metadata.AdditionalProperties.ContainsKey("Visible")
                && bool.TryParse(template.Metadata.AdditionalProperties["Visible"], out visible))
            {
                item.Visible = visible;
            }

            return item;
        }

        /// <summary>
        /// Map the information sent back by Campaign into a format which can be saved as a file to disk.
        /// </summary>
        /// <param name="rawQueryResponse">Raw response from Campaign</param>
        /// <returns>Class containing file content and metadata</returns>
        public override Template ParseQueryResponse(string rawQueryResponse)
        {
            var doc = new XmlDocument();
            doc.LoadXml(rawQueryResponse);

            var metadata = new TemplateMetadata
            {
                Schema = InternalName.Parse(IncludeView.Schema),
                Name = new InternalName(null, doc.DocumentElement.Attributes["name"].InnerText),
                Label = doc.DocumentElement.Attributes["label"].InnerText,
            };

            var visibleAttribute = doc.DocumentElement.Attributes["visible"];
            if (visibleAttribute != null)
            {
                metadata.AdditionalProperties.Add("Visible", (visibleAttribute.InnerText == "1").ToString());
            }

            var codeNode = doc.DocumentElement.SelectSingleNode("source/text");
            var rawCode = codeNode == null
                          ? string.Empty
                          : codeNode.InnerText;

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
