using System;
using System.Collections.Generic;
using System.Xml;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.Templates.Common.Mappings
{
    public abstract class EntityMapping : IMapping
    {
        #region Fields

        private readonly string[] _queryFields = { "@xtkschema" };

        #endregion

        #region Properties

        protected abstract string Schema { get; }

        public abstract Type MappingFor { get; }

        public virtual IEnumerable<string> QueryFields { get { return _queryFields; } }

        #endregion

        #region Methods

        public virtual IPersistable GetPersistableItem(Template template)
        {
            return new Form
            {
                Name = template.Metadata.Name,
                Label = template.Metadata.Label,
                RawXml = template.Code,
            };
        }

        public virtual Template ParseQueryResponse(string rawQueryResponse)
        {
            var doc = new XmlDocument();
            doc.LoadXml(rawQueryResponse);

            var metadata = new TemplateMetadata
            {
                Schema = InternalName.Parse(JavaScriptCode.Schema),
                Name = new InternalName(doc.DocumentElement.Attributes["namespace"].InnerText, doc.DocumentElement.Attributes["name"].InnerText),
                Label = doc.DocumentElement.Attributes["label"].InnerText,
            };

            var rawCode = doc.OuterXml;
            return new Template
            {
                Code = rawCode,
                Metadata = metadata,
                FileExtension = FileTypes.Xml,
            };
        }

        #endregion
    }
}
