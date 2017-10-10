using System.Xml;
using Cwm.AdobeCampaign.WebServices.Model.Abstract;

namespace Cwm.AdobeCampaign.WebServices.Model
{
    /// <summary>
    /// Class representing a personalisation block (nms:includeView).
    /// </summary>
    [Schema(EntitySchema)]
    public class IncludeView : Persistable, IPersistable
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string EntitySchema = "nms:includeView";

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
        /// The raw code of the text version as a string.
        /// </summary>
        public string TextCode { get; set; }

        /// <summary>
        /// The raw code of the html version as a string.
        /// </summary>
        public string HtmlCode { get; set; }

        /// <summary>
        /// Flag indicating whether the content of this personalisation block depends on the format of the content in which it's included.
        /// </summary>
        public bool? VariesByFormat { get; set; }

        /// <summary>
        /// Flag indicating whether this personalisation block is included in customisation menua.
        /// </summary>
        public bool? IncludeInCustomisationMenus { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the dataa into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <param name="ownerDocument">Document to create the xml element from</param>
        /// <returns>Xml element containing all the properties to update</returns>
        public virtual XmlElement GetXmlForPersist(XmlDocument ownerDocument)
        {
            var element = GetBaseXmlForPersist(ownerDocument, "@name");
            element.AppendAttribute("name", Name.Name);

            if (Label != null)
            {
                element.AppendAttribute("label", Label);
            }

            if (FolderId != null)
            {
                var folderElement = element.AppendChild("folder");
                folderElement.AppendAttribute("id", FolderId.ToString());
            }

            var sourceElement = element.AppendChild("source");
            if(VariesByFormat != null)
            {
                sourceElement.AppendAttribute("dependOnFormat", VariesByFormat.Value.ToString().ToLower());
            }

            if (TextCode != null)
            {
                var textElement = sourceElement.AppendChild("text");
                var textCData = ownerDocument.CreateCDataSection(TextCode);
                textElement.AppendChild(textCData);
            }

            if (HtmlCode != null)
            {
                var htmlElement = sourceElement.AppendChild("html");
                var htmlCData = ownerDocument.CreateCDataSection(HtmlCode);
                htmlElement.AppendChild(htmlCData);
            }

            if (IncludeInCustomisationMenus != null)
            {
                element.AppendAttribute("visible", IncludeInCustomisationMenus.Value.ToString().ToLower());
            }

            return element;
        }

        #endregion
    }
}
