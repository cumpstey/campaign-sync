using System;
using System.Collections.Generic;
using System.Xml;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.Templates.Common.Mappings
{
    public class IncludeViewMapping : IMapping
    {
        #region Fields

        private readonly string[] _queryFields = { "source/text", "source/html" };

        #endregion

        #region Properties

        protected string Schema { get { return IncludeView.Schema; } }

        public Type MappingFor { get { return typeof(IncludeView); } }

        public IEnumerable<string> QueryFields { get { return _queryFields; } }

        #endregion

        #region Methods

        public IPersistable GetPersistableItem(Template template)
        {
            return new IncludeView
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
                Name = new InternalName(null, doc.DocumentElement.Attributes["name"].InnerText),
                Label = doc.DocumentElement.Attributes["label"].InnerText,
            };

            var codeNode = doc.DocumentElement.SelectSingleNode("source/text");
            var rawCode = codeNode == null
                          ? string.Empty
                          : codeNode.InnerText;

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
