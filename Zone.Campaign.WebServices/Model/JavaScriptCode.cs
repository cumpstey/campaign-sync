using System.Xml;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    /// <summary>
    /// Class representing a JavaScript file (xtk:javascript).
    /// </summary>
    [Schema(Schema)]
    public class JavaScriptCode : Persistable, IPersistable
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string Schema = "xtk:javascript";

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
        /// The raw xml of the code file as a string.
        /// </summary>
        public string Code { get; set; }

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

            if (Code != null)
            {
                var codeElement = element.AppendChild("data");
                var codeCData = ownerDocument.CreateCDataSection(Code);
                codeElement.AppendChild(codeCData);
            }

            return element;
        }

        #endregion
    }
}
