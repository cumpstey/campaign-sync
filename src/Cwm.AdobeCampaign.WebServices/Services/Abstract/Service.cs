using System.Xml.Linq;

namespace Cwm.AdobeCampaign.WebServices.Services.Abstract
{
    /// <summary>
    /// Base class for web service wrapper classes.
    /// </summary>
    public abstract class Service
    {
        #region Helpers

        /// <summary>
        /// Create an XML document containing a barebones SOAP envelope.
        /// </summary>
        /// <param name="serviceName">Name of the SOAP service</param>
        /// <param name="serviceNs">Namespace of the SOAP service</param>
        /// <returns>The XML element which should be populated with the details of the request</returns>
        protected XDocument CreateServiceRequest(string serviceName, XNamespace serviceNs)
        {
            XNamespace soapNs = Soap.XmlNamespace;
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(soapNs + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "soapenv", soapNs),
                    new XElement(soapNs + "Header"),
                    new XElement(soapNs + "Body",
                        new XElement(serviceNs + serviceName,
                            new XAttribute(XNamespace.Xmlns + "urn", serviceNs),
                            new XElement(serviceNs + "sessiontoken")))));
            return doc;
        }

        /// <summary>
        /// Selects the xml element holding the service information from the xml document of the soap request.
        /// </summary>
        /// <param name="requestDoc">Xml document of soap request</param>
        /// <param name="serviceName">Name of the service</param>
        /// <param name="serviceNs">Namespace of the service</param>
        protected XElement GetServiceElement(XDocument requestDoc, string serviceName, XNamespace serviceNs)
        {
            XNamespace soapNs = Soap.XmlNamespace;
            return requestDoc.Element(soapNs + "Envelope").Element(soapNs + "Body").Element(serviceNs + serviceName);
        }

        #endregion
    }
}
