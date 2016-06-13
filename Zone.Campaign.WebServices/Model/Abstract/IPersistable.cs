using System.Xml;

namespace Zone.Campaign.WebServices.Model.Abstract
{
    public interface IPersistable
    {
        XmlElement GetXmlForPersist(XmlDocument ownerDocument);
    }
}
