using Ionic.Zip;
using System;
using System.IO;
using System.Xml;

namespace Zone.Campaign.WebServices.Services.Abstract
{
    /// <summary>
    /// Base class for services which zip requests before sending them to Campaign.
    /// </summary>
    public abstract class ZippedService : Service
    {
        #region Helpers

        /// <summary>
        /// Zip and base64 encode the SOAP request.
        /// </summary>
        /// <param name="queryElement">XML node containing the SOAP request</param>
        /// <param name="queryName">Name of the query</param>
        /// <returns>Zipped and encoded request</returns>
        public string ZipAndEncodeQuery(XmlNode queryElement, string queryName)
        {
            byte[] zippedQuery;
            using (var stream = new MemoryStream())
            using (var zip = new ZipFile())
            {
                zip.AddEntry(queryName + ".xml", queryElement.OuterXml);
                zip.Save(stream);
                zippedQuery = stream.ToArray();
            }

            var encodedQuery = Convert.ToBase64String(zippedQuery);
            return encodedQuery;
        }

        #endregion
    }
}
