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

        public override IEnumerable<string> QueryFields { get { return _queryFields; } }

        #endregion

        #region Methods

        public override IPersistable GetPersistableItem(Template template)
        {
            return new JavaScriptTemplate
            {
                Name = template.Metadata.Name,
                Label = template.Metadata.Label,
                Code = template.Code.Trim(),
            };
        }

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
