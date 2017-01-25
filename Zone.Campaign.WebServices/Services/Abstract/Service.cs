using System.Xml;
using Zone.Campaign.WebServices.Security;

namespace Zone.Campaign.WebServices.Services.Abstract
{
    public abstract class Service
    {
        #region Helpers

        protected XmlNode CreateServiceRequest(string serviceName, string serviceNs, Tokens tokens)
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

        protected XmlNode SelectSingleNode(XmlNode node, string xpath, string serviceNs)
        {
            var nsmgr = new XmlNamespaceManager(node.OwnerDocument.NameTable);
            nsmgr.AddNamespace("soap", Soap.XmlNamespace);
            nsmgr.AddNamespace("urn", serviceNs);

            var responseElement = node.SelectSingleNode(xpath, nsmgr);
            return responseElement;
        }

        #endregion
    }
}
