using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Cwm.AdobeCampaign.Templates.Services;
using System;

namespace Cwm.AdobeCampaign.Templates.Tests
{
    /// <summary>
    /// An implementation of the <see cref="IFileProvider"/> interface based on embedded resources.
    /// This has been built specifically to aid testing, without an intention of using it in production code.
    /// </summary>
    public class EmbeddedResourceFileProvider : IFileProvider
    {
        #region Fields

        private Assembly _assembly;

        private Type _callingType;

        #endregion

        #region Constructor

        public EmbeddedResourceFileProvider(Type callingType)
        {
            _callingType = callingType;
            _assembly = callingType.Assembly;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Combines two strings into a path.
        /// </summary>
        /// <param name="path1">The first path to combine</param>
        /// <param name="path2">The second path to combine</param>
        /// <returns>The combined path</returns>
        public string CombinePaths(string path1, string path2)
        {
            return $"{path1}.{path2}";
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check</param>
        /// <returns>Whether the file exists</returns>
        public bool FileExists(string path)
        {
            return ResourceExists(path);
        }

        /// <summary>
        /// Returns the directory information for the specified path string.
        /// </summary>
        /// <param name="path">The path of a file or directory</param>
        /// <returns>Directory path</returns>
        public string GetDirectoryName(string path)
        {
            return path.Substring(0, path.LastIndexOf("."));
        }

        /// <summary>
        /// Returns the full resource name for the specified name fragment.
        /// </summary>
        /// <param name="path">The name of the resource for which to return the full name</param>
        /// <returns>The full name of the resource</returns>
        public string GetFullPath(string path)
        {
            return GetResourceName(path);
        }

        /// <summary>
        /// Returns the text content of the specified resource.
        /// </summary>
        /// <param name="path">The full name of the resource</param>
        /// <returns>The resource content as a string</returns>
        public string ReadAllText(string path)
        {
            return GetEmbeddedResource(path);
        }

        #endregion

        #region Helpers

        private string GetResourceName(string name)
        {
            var rootNamespace = _assembly.GetName().Name;

            // If path is already rooted, return as is.
            if (name.StartsWith(rootNamespace))
            {
                return name;
            }

            var namespaceFragment = Regex.Replace(_callingType.FullName.Replace(rootNamespace, null).TrimStart('.'), "Tests$", string.Empty);

            var resourceName = $"{rootNamespace}.TestResources.{namespaceFragment}.{name}";
            return resourceName;
        }

        protected bool ResourceExists( string fullName)
        {
            var resourceName = fullName + ".txt";
            var resources = _assembly.GetManifestResourceNames();
            return resources.Contains(resourceName);
        }

        protected string GetEmbeddedResource(string fullName)
        {
            if (!ResourceExists(fullName))
            {
                return null;
            }

            var resourceName = fullName + ".txt";

            using (Stream stream = _assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion
    }
}
