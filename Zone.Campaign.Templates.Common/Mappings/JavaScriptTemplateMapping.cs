using System;
using System.Collections.Generic;
using System.Xml;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.Templates.Common.Mappings
{
    public class JavaScriptTemplateMapping : IMapping
    {
        #region Fields

        private readonly string[] _queryFields = { "@entitySchema", "code" };

        #endregion

        #region Properties

        protected string Schema { get { return JavaScriptTemplate.Schema; } }

        public Type MappingFor { get { return typeof(JavaScriptTemplate); } }

        public IEnumerable<string> QueryFields { get { return _queryFields; } }

        #endregion

        #region Methods

        public IPersistable GetPersistableItem(Template template)
        {
            return new JavaScriptTemplate
            {
                Name = template.Metadata.Name,
                Label = template.Metadata.Label,
                Code = template.Code,
            };
        }

        public Template ParseQueryResponse(string rawQueryResponse)
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

            // TODO somehow deal with text versions of these templates.
            return new Template
            {
                Code = rawCode,
                Metadata = metadata,
                FileExtension = FileTypes.Html,
            };
        }

        #endregion
    }
}
