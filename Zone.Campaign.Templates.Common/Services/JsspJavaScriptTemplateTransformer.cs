using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;
using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services
{
    /// <summary>
    /// Provides functions to transform JavaScript Server Pages code before it's uploaded to Campaign.
    /// </summary>
    public class JsspJavaScriptTemplateTransformer : ITemplateTransformer
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(JsspJavaScriptTemplateTransformer));

        private static readonly Regex IncludeRegex = new Regex(@"<%--@include\s*(?<path>.*?)\s*@(?<flags>[t]*)--%>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex CodeFileRegex = new Regex(@"^\s*<%(?<content>.*)%>\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Methods

        /// <summary>
        /// Transforms JavaSript Server Pages code so that it can be uploaded to Campaign.
        /// Allows the content of local JSSP files to be transformed for use in Campaign.
        /// There is no reverse method, so JavaScript Server Pages files cannot be directly downloaded from Campaign
        /// into the format in which they are stored locally.
        /// </summary>
        /// <param name="template">Source content</param>
        /// <param name="parameters">Parameters to determine transform behaviour</param>
        /// <returns>Transformed JavaScript Server Pages content</returns>
        public IEnumerable<Template> Transform(Template template, TransformParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrEmpty(parameters.OriginalFileName))
            {
                throw new ArgumentException("Original file name must be provided.", nameof(parameters.OriginalFileName));
            }

            var workingDirectory = Path.GetDirectoryName(parameters.OriginalFileName);

            template.Code = ProcessIncludes(template.Code, workingDirectory);
            return new[] { template };
        }

        #endregion
        
        #region Helpers

        /// <summary>
        /// Processes specifically the &lt;%--@include @--%&gt; comments which contain include directives
        /// </summary>
        private static string ProcessIncludes(string input, string workingDirectory)
        {
            var output = input;
            var matches = IncludeRegex.Matches(output);
            foreach (var match in matches.Cast<Match>().Reverse())
            {
                var path = match.Groups["path"].Value;
                var flags = match.Groups["flags"].Value.Distinct().ToArray();

                var start = match.Captures[0].Index;
                var length = match.Captures[0].Length;

                // Remove insert directive.
                output = output.Remove(start, length);

                // Insert file content.
                var fullPath = Path.GetFullPath(Path.Combine(workingDirectory, path));
                if (File.Exists(fullPath))
                {
                    var fileContent = File.ReadAllText(fullPath);

                    // If trim flag is specified, trim file content.
                    if (flags.Contains('t'))
                    {
                        fileContent = fileContent.Trim();
                    }

                    var includeContent = ProcessIncludes(fileContent, Path.GetDirectoryName(fullPath));
                    output = output.Insert(start, includeContent);
                }
                else
                {
                    Log.Warn($"Cannot include file in template, as it does not exist: {fullPath}.");
                }
            }

            return output;
        }

        #endregion
    }
}
