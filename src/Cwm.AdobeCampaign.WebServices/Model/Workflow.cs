using System.Linq;
using System.Xml;
using Cwm.AdobeCampaign.WebServices.Model.Abstract;
using System.Xml.Linq;

namespace Cwm.AdobeCampaign.WebServices.Model
{
    /// <summary>
    /// Class representing a JavaScript file (xtk:javascript).
    /// </summary>
    [Schema(EntitySchema)]
    public class Workflow : Persistable, IPersistable
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string EntitySchema = "xtk:workflow";

        #endregion

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
        /// Id of the folder this entity is stored in.
        /// </summary>
        public int? FolderId { get; set; }

        /// <summary>
        /// The raw xml data as a string.
        /// </summary>
        public string RawXml { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the dataa into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <param name="ownerDocument">Document to create the xml element from</param>
        /// <returns>Xml element containing all the properties to update</returns>
        public virtual XElement GetXmlForPersist(XDocument ownerDocument)
        {
            var element = GetBaseXmlForPersist(ownerDocument, "@internalName");
            element.AppendAttribute("internalName", Name.Name);

            if (Label != null)
            {
                element.AppendAttribute("label", Label);
            }

            if (FolderId != null)
            {
                var folderElement = element.AppendChild("folder");
                folderElement.AppendAttribute("id", FolderId.ToString());
            }

            var contentDoc = new XmlDocument();
            contentDoc.LoadXml(RawXml);
            foreach (var attribute in contentDoc.DocumentElement.Attributes.Cast<XmlAttribute>().ToArray())
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
