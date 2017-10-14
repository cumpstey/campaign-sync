namespace Cwm.AdobeCampaign.Sync.Data
{
    /// <summary>
    /// Class to hold all the metadata for an image which, along with the image file itself,
    /// are necessary in order to create it as a file resource.
    /// </summary>
    public class ImageData
    {
        /// <summary>
        /// Local file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Name of the folder in Campaign it should be created in.
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// Internal name of the file resource.
        /// </summary>
        public string InternalName { get; set; }

        /// <summary>
        /// Label of the file resource.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Alt text of the image.
        /// </summary>
        public string Alt { get; set; }
    }
}
