using System;

namespace Zone.Campaign.Sync
{
    public class FileExtensionAttribute : Attribute
    {
        #region Constructor

        public FileExtensionAttribute(string fileExtension)
        {
            FileExtension = fileExtension;
        }

        #endregion

        #region Properties

        public string FileExtension { get; private set; }

        #endregion
    }
}
