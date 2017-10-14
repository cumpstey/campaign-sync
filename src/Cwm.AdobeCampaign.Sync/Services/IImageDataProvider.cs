using System.Collections.Generic;
using Cwm.AdobeCampaign.Sync.Data;

namespace Cwm.AdobeCampaign.Sync.Services
{
    /// <summary>
    /// Contains functions for reading and generating image data files.
    /// </summary>
    public interface IImageDataProvider
    {
        #region Methods

        /// <summary>
        /// Read all the image data recursively from the image data files in a directory tree.
        /// </summary>
        /// <param name="directoryPath">Root directory path</param>
        /// <returns>Colelction of image data</returns>
        IEnumerable<ImageData> GetData(string filePath);

        /// <summary>
        /// Generate image data files, containing stub data for all images in a folder.
        /// Updates any existing incomplete files with rows for any new images found.
        /// </summary>
        /// <param name="directoryPath">Root directory path</param>
        /// <param name="recursive">Whether to generate files recursively in descendent directories</param>
        /// <param name="extensions">List of file extensions recognised as image files</param>
        void GenerateDataFile(string directoryPath, bool recursive, IEnumerable<string> extensions);

        #endregion
    }
}
