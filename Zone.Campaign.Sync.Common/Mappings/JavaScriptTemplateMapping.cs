using System;
using System.Collections.Generic;
using System.Xml;
using Zone.Campaign.Templates;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.Sync.Mappings
{
    public class JavaScriptTemplateMapping : Mapping<JavaScriptTemplate>
    {
        #region Fields

        private readonly string[] _queryFields = { "@name", "@label", "@entitySchema", "code" };

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
            return new JavaScriptTemplate
            {
                Name = template.Metadata.Name,
                Label = template.Metadata.Label,
                Code = template.Code.Trim(),
            };
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
                Schema = InternalName.Parse(JavaScriptTemplate.Schema),
                Name = new InternalName(doc.DocumentElement.Attributes["namespace"].InnerText, doc.DocumentElement.Attributes["name"].InnerText),
                Label = doc.DocumentElement.Attributes["label"].InnerText,
            };

            var codeNode = doc.DocumentElement.SelectSingleNode("code");
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
