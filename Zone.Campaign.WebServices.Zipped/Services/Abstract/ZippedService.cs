using Ionic.Zip;
using System;
using System.IO;
using System.Xml;

namespace Zone.Campaign.WebServices.Services.Abstract
{
    public abstract class ZippedService : Service
    {
        #region Helpers

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
