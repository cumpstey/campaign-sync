using System.Linq;
using System.Xml;
using Zone.Campaign;

namespace Zone.Campaign.WebServices.Model.Abstract
{
    /// <summary>
    /// Class representing an entity (xtk:entity) - a base class within Campaign for xml-based classes.
    /// </summary>
    public abstract class Entity : Persistable, IPersistable
    {
        #region Properties

        /// <summary>
        /// Internal name, combining namespace and name.
        /// </summary>
        public InternalName Name { get; set; }

        /// <summary>
        /// Label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The raw xml of the code file as a string.
        /// </summary>
        public string RawXml { get; set; }

        #endregion

        #region Methods
        
        /// <summary>
        /// Formats the data into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <param name="ownerDocument">Document to create the xml element from</param>
        /// <returns>Xml element containing all the properties to update</returns>
        public virtual XmlElement GetXmlForPersist(XmlDocument ownerDocument)
        {
            // Official documentation (https://docs.campaign.adobe.com/doc/AC6.1/en/CFG_API_Data_oriented_APIs.html)
            // suggests that the _key attribute is necessary. In fact, not only is it not necessary, but it under
            // some circumstances breaks the schema: if the schema represents _only_ soap methods, not a database
            // table, then including the _key attribute in a persist request will mean that the schema is left in
            // a state which causes the database update process to fail.
            ////var element = GetBaseXmlForPersist(ownerDocument, "@namespace, @name");

            var element = GetBaseXmlForPersist(ownerDocument, null);
            element.AppendAttribute("namespace", Name.Namespace);
            element.AppendAttribute("name", Name.Name);

            if (Label != null)
            {
                element.AppendAttribute("label", Label);
            }

            var contentDoc = new XmlDocument();
            contentDoc.LoadXml(RawXml);
            foreach(var attribute in contentDoc.DocumentElement.Attributes.Cast<XmlAttribute>().ToArray())
            {
                element.AppendAttribute(attribute.Name, attribute.Value);
            }

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
