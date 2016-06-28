using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zone.Campaign.Sync
{
    public static class ImageHelper
    {
        #region Fields

        public static readonly IDictionary<string, string> ImageMimeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {".gif", "image/gif"},
            {".jpg", "image/jpeg"},
            {".png", "image/png"},
        };

        #endregion

        #region Properties

        public static IEnumerable<string> PermittedExtensions { get { return ImageMimeMappings.Select(i => i.Key); } }

        #endregion

        #region Methods

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
