using System;
using System.Collections.Generic;
using System.Xml;
using Zone.Campaign.Templates;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.Sync.Mappings
{
    public class QueryFilterMapping : Mapping<QueryFilter>
    {
        #region Fields

        private readonly string[] _queryFields = { "@name", "@label", "@schema", "data" };

        private readonly IEnumerable<string> _attributesToKeep = new[] { "schema" };

        #endregion

        #region Properties

        public override IEnumerable<string> QueryFields { get { return _queryFields; } }

        #endregion

        #region Methods

        public override IPersistable GetPersistableItem(Template template)
        {
            return new QueryFilter
            {
                Name = template.Metadata.Name,
                Label = template.Metadata.Label,
                Data = template.Code,
            };
        }

        public override Template ParseQueryResponse(string rawQueryResponse)
        {
            var doc = new XmlDocument();
            doc.LoadXml(rawQueryResponse);

            var metadata = new TemplateMetadata
            {
                Schema = InternalName.Parse(Schema),
                Name = new InternalName(null, doc.DocumentElement.Attributes["name"].InnerText),
                Label = doc.DocumentElement.Attributes["label"].InnerText,
            };

            doc.DocumentElement.RemoveAllAttributesExcept(_attributesToKeep);

            return new Template
            {
                Code = doc.DocumentElement.OuterXml,
                Metadata = metadata,
                FileExtension = FileTypes.Xml,
            };
        }

        #endregion
    }
}
