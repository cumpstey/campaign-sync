using System.Xml;
using Zone.Campaign;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    [Schema(Schema)]
    public class JavaScriptTemplate : Persistable, IPersistable
    {
        #region Fields

        public const string Schema = "xtk:jst";

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

            if (Label != null)
            {
                element.AppendAttribute("label", Label);
            }

            if (Code != null)
            {
                var codeElement = element.AppendChild("code");
                var codeCData = ownerDocument.CreateCDataSection(Code);
                codeElement.AppendChild(codeCData);
            }

            return element;
        }

        #endregion
    }
}
