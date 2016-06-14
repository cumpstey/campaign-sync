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

        public string TextCode { get; set; }

        #endregion

        #region Methods

        public virtual XmlElement GetXmlForPersist(XmlDocument ownerDocument)
        {
            var element = GetBaseXmlForPersist(ownerDocument, "@name");
            element.AppendAttribute("name", Name.Name);

            if (Label != null)
            {
                element.AppendAttribute("label", Label);
            }

            if (TextCode != null)
            {
                var sourceElement = element.AppendChild("source");
                var textElement = element.AppendChild("text");
                var textCData = ownerDocument.CreateCDataSection(TextCode);
                textElement.AppendChild(textCData);
            }

            return element;
        }

        #endregion
    }
}
