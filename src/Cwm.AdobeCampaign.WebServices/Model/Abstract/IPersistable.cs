using System.Xml;
using System.Xml.Linq;

namespace Cwm.AdobeCampaign.WebServices.Model.Abstract
{
    /// <summary>
    /// Interface implemented by classes representing objects which are persistable in Adobe Campaign.
    /// </summary>
    public interface IPersistable
    {
        /// <summary>
        /// Formats the data into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <returns>Xml element containing all the properties to update</returns>
        XElement GetXmlForPersist();
    }
}
