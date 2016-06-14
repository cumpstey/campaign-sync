using System.Linq;
using System.Xml;
using Zone.Campaign;

namespace Zone.Campaign.WebServices.Model.Abstract
{
    public abstract class Entity : Persistable, IPersistable
    {
        #region Properties

        public InternalName Name { get; set; }

        public string Label { get; set; }

        public string RawXml { get; set; }

        #endregion

        #region Methods

        public XmlElement GetXmlForPersist(XmlDocument ownerDocument)
        {
            var element = GetBaseXmlForPersist(ownerDocument, "@namespace, @name");
            element.AppendAttribute("namespace", Name.Namespace);
            element.AppendAttribute("name", Name.Name);

            if (Label != null)
            {
                element.AppendAttribute("label", Label);
            }

            var contentDoc = new XmlDocument();
            contentDoc.LoadXml(RawXml);
            foreach (var child in contentDoc.DocumentElement.ChildNodes.Cast<XmlNode>().ToArray())
            {
                var importedNode = ownerDocument.ImportNode(child, true);
                element.AppendChild(importedNode);                
            }

            return element;
        }

        #endregion
    }
}
