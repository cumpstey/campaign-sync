using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zone.Campaign.Sync
{
    /// <summary>
    /// Common properties and functions relating to images.
    /// </summary>
    public static class ImageHelper
    {
        #region Fields

        /// <summary>
        /// A dictionary mapping image file extensions with mime types.
        /// </summary>
        public static readonly IDictionary<string, string> ImageMimeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {".gif", "image/gif"},
            {".jpg", "image/jpeg"},
            {".png", "image/png"},
        };

        #endregion

        #region Properties

        /// <summary>
        /// A collection of file extensions recognised as image files which can be uploaded to Campaign.
        /// </summary>
        public static IEnumerable<string> PermittedExtensions { get { return ImageMimeMappings.Select(i => i.Key); } }

        #endregion

        #region Methods

        /// <summary>
        /// Get the mime type for a given file extension.
        /// </summary>
        /// <param name="extension">File extension</param>
        /// <returns>Mime type</returns>
        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = string.Concat(".", extension);
            }

            string mimeType;
            return ImageMimeMappings.TryGetValue(extension, out mimeType) ? mimeType : null;
        }

        #endregion
    }
}
