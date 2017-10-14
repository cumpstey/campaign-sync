using System;
using System.Xml;
using System.Xml.Linq;

namespace Cwm.AdobeCampaign.WebServices.Model
{
    /// <summary>
    /// Class holding all the necessary information about an image in order to create a file resource (xtk:fileRes) record.
    /// </summary>
    public class ImageResource
    {
        #region Properties

        /// <summary>
        /// Name of the folder the file resource record should be created in.
        /// If a file resource of the same name already exists in a different folder, it will not be moved.
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// Information about the file resource.
        /// </summary>
        public FileResource FileRes { get; set; }

        /// <summary>
        /// Mime type of the image.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// File name of the image.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// MD5 hash of the image.
        /// </summary>
        public string Md5 { get; set; }

        /// <summary>
        /// Base64 encoded image file content.
        /// </summary>
        public string FileContent { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the data into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <returns>Xml element containing all the properties to update</returns>
        public XElement GetXmlForPersist()
        {
            throw new NotImplementedException();

            //var element = ownerDocument.CreateElement("file");
            //element.AppendAttribute("folderName", FolderName);

            //var fileResElement = element.AppendChild("fileRes");
            //fileResElement.AppendAttribute("internalName", FileRes.Name.Name);

            //if (!string.IsNullOrEmpty(FileRes.Label))
            //{
            //    fileResElement.AppendAttribute("label", FileRes.Label);
            //}

            //if (!string.IsNullOrEmpty(FileRes.Alt))
            //{
            //    fileResElement.AppendAttribute("alt", FileRes.Alt);
            //}

            //if (FileRes.Width != null)
            //{
            //    fileResElement.AppendAttribute("width", FileRes.Width.ToString());
            //}

            //if (FileRes.Height != null)
            //{
            //    fileResElement.AppendAttribute("height", FileRes.Height.ToString());
            //}

            //var fileContentElement = element.AppendChild("fileContent");
            //var fileContentCData = ownerDocument.CreateCDataSection(FileContent);
            //fileContentElement.AppendChild(fileContentCData);

            //fileContentElement.AppendAttribute("mimeType", MimeType);
            //fileContentElement.AppendAttribute("md5", Md5);
            //fileContentElement.AppendAttribute("fileName", FileName);

            //return element;
        }

        #endregion
    }
}
