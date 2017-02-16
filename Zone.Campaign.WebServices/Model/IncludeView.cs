using System.Xml;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    /// <summary>
    /// Class representing a personalisation block (nms:includeView).
    /// </summary>
    [Schema(Schema)]
    public class IncludeView : Persistable, IPersistable
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string Schema = "nms:includeView";

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
        /// The raw xml of the code of the text version as a string.
        /// </summary>
        public string TextCode { get; set; }

        /// <summary>
        /// Flag indicating whether this personalisation block is visible in selection lists.
        /// </summary>
        public bool? Visible { get; set; }

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

            if (TextCode != null)
            {
                var sourceElement = element.AppendChild("source");
                var textElement = sourceElement.AppendChild("text");
                var textCData = ownerDocument.CreateCDataSection(TextCode);
                textElement.AppendChild(textCData);
            }

            if (Visible != null)
            {
                element.AppendAttribute("visible", Visible.Value.ToString().ToLower());
            }

            return element;
        }

        #endregion
    }
}
