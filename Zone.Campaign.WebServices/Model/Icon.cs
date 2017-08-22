using System;
using System.Xml;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    /// <summary>
    /// Class representing an image (xtk:image).
    /// </summary>
    [Schema(EntitySchema)]
    public class Icon : Persistable, IPersistable
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string EntitySchema = "xtk:image";

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
        /// Base64 encoded image data.
        /// </summary>
        public string FileContent { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the dataa into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <param name="ownerDocument">Document to create the xml element from</param>
        /// <returns>Xml element containing all the properties to update</returns>
        public virtual XmlElement GetXmlForPersist(XmlDocument ownerDocument)
        {
            var element = GetBaseXmlForPersist(ownerDocument, "@namespace, @name");
            element.AppendAttribute("namespace", Name.Namespace);
            element.AppendAttribute("name", Name.Name);

            if (Label != null)
            {
                element.AppendAttribute("label", Label);
            }

            if (FileContent != null)
            {
                var dataElement = element.AppendChild("data");
                var dataCData = ownerDocument.CreateCDataSection(FileContent);
                dataElement.AppendChild(dataCData);
            }

            return element;
        }

        #endregion
    }
}
