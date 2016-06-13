using System.Xml;
using Zone.Campaign;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    [Schema(Schema)]
    public class JavaScriptCode : Persistable, IPersistable
    {
        #region Fields

        public const string Schema = "xtk:javascript";

        #endregion

        #region Properties

        public InternalName Name { get; set; }

        public string Label { get; set; }

        public string Data { get; set; }

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

            var codeElement = element.AppendChild("data");
            var codeCData = ownerDocument.CreateCDataSection(Data);
            codeElement.AppendChild(codeCData);

            return element;
        }

        #endregion
    }
}
