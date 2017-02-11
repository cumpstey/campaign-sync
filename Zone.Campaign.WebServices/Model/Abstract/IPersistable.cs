using System.Xml;

namespace Zone.Campaign.WebServices.Model.Abstract
{
    /// <summary>
    /// Interface implemented by classes representing objects which are persistable in Adobe Campaign.
    /// </summary>
    public interface IPersistable
    {
        /// <summary>
        /// Formats the data into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <param name="ownerDocument">Document to create the xml element from</param>
        /// <returns>Xml element containing all the properties to update</returns>
        XmlElement GetXmlForPersist(XmlDocument ownerDocument);
    }
}
