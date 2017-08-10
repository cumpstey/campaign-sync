using System.Xml;

namespace Zone.Campaign.WebServices.Services.Abstract
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
        protected XmlElement CreateServiceRequest(string serviceName, string serviceNs)
        {
            var doc = new XmlDocument();
            doc.LoadXml(string.Format(@"<soapenv:Envelope xmlns:soapenv=""{0}""/>", Soap.XmlNamespace));

            doc.DocumentElement.AppendChild("soapenv:Header", Soap.XmlNamespace);
            var bodyElement = doc.DocumentElement.AppendChild("soapenv:Body", Soap.XmlNamespace);

            var serviceElement = bodyElement.AppendChild(string.Concat("urn:", serviceName), serviceNs);

            // The session token can be added either in the request body or in header as a cookie.
            // We add it as a header, to make it easier to deal with queuing requests for later processing.
            // We still need the empty node here.
            serviceElement.AppendChild("urn:sessiontoken", serviceNs);

            return serviceElement;
        }

        /// <summary>
        /// Select a single node by xpath from the node supplied.
        /// </summary>
        /// <param name="node">Source node</param>
        /// <param name="xpath">Xpath defining the node to select</param>
        /// <param name="serviceNs">Namespace of the SOAP service (with urn prefix)</param>
        /// <returns>Selected XML node</returns>
        protected XmlNode SelectSingleNode(XmlNode node, string xpath, string serviceNs)
        {
            var nsmgr = new XmlNamespaceManager(node.OwnerDocument.NameTable);
            nsmgr.AddNamespace("soap", Soap.XmlNamespace);
            nsmgr.AddNamespace("urn", serviceNs);

            var selected = node.SelectSingleNode(xpath, nsmgr);
            return selected;
        }

        /// <summary>
        /// Select nodes by xpath from the node supplied.
        /// </summary>
        /// <param name="node">Source node</param>
        /// <param name="xpath">Xpath defining the node to select</param>
        /// <param name="serviceNs">Namespace of the SOAP service (with urn prefix)</param>
        /// <returns>Selected XML node</returns>
        protected XmlNodeList SelectNodes(XmlNode node, string xpath, string serviceNs)
        {
            var nsmgr = new XmlNamespaceManager(node.OwnerDocument.NameTable);
            nsmgr.AddNamespace("soap", Soap.XmlNamespace);
            nsmgr.AddNamespace("urn", serviceNs);

            var selected = node.SelectNodes(xpath, nsmgr);
            return selected;
        }

        #endregion
    }
}
