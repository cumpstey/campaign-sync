using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cwm.AdobeCampaign.Templates.Services
{
    public class FileSystemFileProvider
    {
        /// <summary>
        /// Combines two strings into a path.
        /// </summary>
        /// <param name="path1">The first path to combine</param>
        /// <param name="path2">The second path to combine</param>
        /// <returns>The combined path</returns>
        public string CombinePaths(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check</param>
        /// <returns>Whether the file exists</returns>
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Returns the directory information for the specified path string.
        /// </summary>
        /// <param name="path">The path of a file or directory</param>
        /// <returns>Directory path</returns>
        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Returns the absolute path for the specified path string.
        /// </summary>
        /// <param name="path">The file or directory for which to obtain absolute path information</param>
        /// <returns>The absolute path</returns>
        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Opens a text file, reads all the lines of the file, and then closes the file.
        /// </summary>
        /// <param name="path">The file to open for reading</param>
        /// <returns>The file content as a string</returns>
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
