using System;
using System.Collections.Generic;
using System.Xml;
using Zone.Campaign.Templates;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.Sync.Mappings.Abstract
{
    public abstract class EntityMapping<T> : Mapping<T>
        where T : Entity, new()
    {
        #region Fields

        private readonly string[] _queryFields = { "@name", "@label", "data" };

        #endregion

        #region Properties

        public override IEnumerable<string> QueryFields { get { return _queryFields; } }

        public virtual IEnumerable<string> AttributesToKeep { get { return new string[0]; } }

        #endregion

        #region Methods

        public override IPersistable GetPersistableItem(Template template)
        {
            return new T
            {
                Name = template.Metadata.Name,
                Label = template.Metadata.Label,
                RawXml = template.Code,
            };
        }

        public override Template ParseQueryResponse(string rawQueryResponse)
        {
            var doc = new XmlDocument();
            doc.LoadXml(rawQueryResponse);

            var metadata = new TemplateMetadata
            {
                Schema = InternalName.Parse(Schema),
                Name = new InternalName(doc.DocumentElement.Attributes["namespace"].InnerText, doc.DocumentElement.Attributes["name"].InnerText),
                Label = doc.DocumentElement.Attributes["label"].InnerText,
            };

            doc.DocumentElement.RemoveAllAttributesExcept(AttributesToKeep);
            doc.DocumentElement.RemoveChild("createdBy");
            doc.DocumentElement.RemoveChild("modifiedBy");

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
