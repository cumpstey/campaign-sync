using System.Xml;
using Zone.Campaign.WebServices.Model.Abstract;
using Zone.Campaign;

namespace Zone.Campaign.WebServices.Model
{
    public class ImageFile
    {
        #region Properties

        public string FolderName { get; set; }

        public FileRes FileRes { get; set; }

        public string MimeType { get; set; }

        public string FileName { get; set; }

        public string Md5 { get; set; }

        public string FileContent { get; set; }

        #endregion

        #region Methods

        public XmlElement GetXmlForPersist(XmlDocument ownerDocument)
        {
            var element = ownerDocument.CreateElement("file");
            element.AppendAttribute("folderName", FolderName);

            var fileResElement = element.AppendChild("fileRes");
            fileResElement.AppendAttribute("internalName", FileRes.Name.Name);

            if (!string.IsNullOrEmpty(FileRes.Label))
            {
                fileResElement.AppendAttribute("label", FileRes.Label);
            }

            if (!string.IsNullOrEmpty(FileRes.Alt))
            {
                fileResElement.AppendAttribute("alt", FileRes.Alt);
            }

            if (FileRes.Width != null)
            {
                fileResElement.AppendAttribute("width", FileRes.Width.ToString());
            }

            if (FileRes.Height != null)
            {
                fileResElement.AppendAttribute("height", FileRes.Height.ToString());
            }

            var fileContentElement = element.AppendChild("fileContent");
            var fileContentCData = ownerDocument.CreateCDataSection(FileContent);
            fileContentElement.AppendChild(fileContentCData);

            fileContentElement.AppendAttribute("mimeType", MimeType);
            fileContentElement.AppendAttribute("md5", Md5);
            fileContentElement.AppendAttribute("fileName", FileName);

            return element;
        }

        #endregion
    }
}
