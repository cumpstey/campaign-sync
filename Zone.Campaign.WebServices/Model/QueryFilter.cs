using System;
using System.Xml;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    [Schema(Schema)]
    public class QueryFilter : Persistable, IPersistable
    {
        #region Fields

        public const string Schema = "xtk:queryFilter";

        #endregion

        #region Properties

        public InternalName Name { get; set; }

        public string Label { get; set; }

        public string Data { get; set; }

        #endregion

        #region Methods

        public virtual XmlElement GetXmlForPersist(XmlDocument ownerDocument)
        {
            var element = GetBaseXmlForPersist(ownerDocument, "@name");
            element.AppendAttribute("name", Name.Name);

            element.AppendAttribute("label", Label);
            element.AppendChildWithValue("data", Data);

            return element;
        }

        #endregion
    }
}
