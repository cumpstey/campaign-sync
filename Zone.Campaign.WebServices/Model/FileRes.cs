using System.Xml;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    /// <summary>
    /// Class representing a file resource (xtk:fileRes).
    /// </summary>
    [Schema(Schema)]
    public class FileRes : Persistable, IPersistable
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string Schema = "xtk:FileRes";

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
        /// Alt text for the image.
        /// </summary>
        public string Alt { get; set; }

        /// <summary>
        /// Width of the image.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Height of the image.
        /// </summary>
        public int? Height { get; set; }

        #endregion

        #region Methods
        
        /// <summary>
        /// Formats the dataa into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <param name="ownerDocument">Document to create the xml element from</param>
        /// <returns>Xml element containing all the properties to update</returns>
        public virtual XmlElement GetXmlForPersist(XmlDocument ownerDocument)
        {
            var element = GetBaseXmlForPersist(ownerDocument, "@internalName");
            element.AppendAttribute("internalName", Name.Name);

            if (!string.IsNullOrEmpty(Label))
            {
                element.AppendAttribute("label", Label);
            }

            if (!string.IsNullOrEmpty(Alt))
            {
                element.AppendAttribute("alt", Label);
            }

            if (Width == null)
            {
                element.AppendAttribute("width", Width.ToString());
            }

            if (Height == null)
            {
                element.AppendAttribute("height", Height.ToString());
            }

            return element;
        }

        #endregion
    }
}
