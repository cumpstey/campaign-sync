using System.Xml;
using Zone.Campaign;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    [Schema(Schema)]
    public class IncludeView : Persistable, IPersistable
    {
        #region Fields

        public const string Schema = "nms:includeView";

        #endregion

        #region Properties

        public InternalName Name { get; set; }

        public string Label { get; set; }

        public string Code { get; set; }

        #endregion

        #region Methods

        public virtual XmlElement GetXmlForPersist(XmlDocument ownerDocument)
        {
            var element = GetBaseXmlForPersist(ownerDocument, "@namespace, @name");
            element.AppendAttribute("namespace", Name.Namespace);
            element.AppendAttribute("name", Name.Name);

            if (!string.IsNullOrEmpty(Label))
            {
                element.AppendAttribute("label", Label);
            }

            var sourceElement = element.AppendChild("source");
            var textElement = element.AppendChild("text");
            var textCData = ownerDocument.CreateCDataSection(Code);
            textElement.AppendChild(textCData);

            return element;
        }

        #endregion
    }
}
