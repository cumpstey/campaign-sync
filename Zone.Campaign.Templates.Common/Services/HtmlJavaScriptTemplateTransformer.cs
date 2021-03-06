﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using log4net;
using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services
{
    /// <summary>
    /// Provides functions to transform HTML code before it's uploaded to Campaign.
    /// </summary>
    public class HtmlJavaScriptTemplateTransformer : ITemplateTransformer
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(HtmlJavaScriptTemplateTransformer));

        private const string FunctionDefinitionFormat = @"function {0} ({1}) {{/-->{2}<!--/}} // end {0}";

        private const string FunctionPrototypeDefinitionFormat = @"{3}.prototype.{0} = function ({1}) {{/-->{2}<!--/}}; // end {0}";

        private const string FunctionPlaceholder = "/*{functions}*/";

        private static readonly Regex FullFunctionRegex = new Regex(@"<!--\{\s*(?<func>[a-z0-9_]+)\s*\((?<args>.*?)\)(\s*\[prototype:(?<prototypeClass>.*?)\])?\s*\}-->(?<code>.*?)<!--\{\s*end\s*\k<func>\s*\}-->", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex IncludeWhenRegex = new Regex(@"<!--@include-(?<when>pre|post)\s*(?<path>.*?)\s*@(?<flags>[ht]*)-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex IncludeRegex = new Regex(@"<%--@include\s*(?<path>.*?)\s*@(?<flags>[t]*)--%>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Methods

        /// <summary>
        /// Transforms HTML code so that it can be uploaded to Campaign.
        /// Allows the content of local HTML files to be transformed into JavaScript Server Pages for use in Campaign.
        /// There is no reverse method, so HTML-based JavaScript Server Pages files cannot be directly downloaded from Campaign
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

            // Process pre-includes.
            template.Code = ProcessIncludes(template.Code, workingDirectory, "pre");

            // Process function definitions.
            template.Code = ProcessFunctionDefinitions(template.Code);

            // Process html.
            template.Code = ProcessHtml(template.Code);

            // Process post-includes.
            template.Code = ProcessIncludes(template.Code, workingDirectory, "post");

            return new[] { template };
        }

        #endregion

        #region Helpers

        private static string ProcessFunctionDefinitions(string input)
        {
            var output = input;
            int found;
            do
            {
                output = ProcessFunctionDefinitionsOnce(output, out found);
            } while (found > 0);

            var replacementIndex = output.IndexOf(FunctionPlaceholder);
            output = output.Remove(replacementIndex, FunctionPlaceholder.Length);

            return output;
        }

        /// <summary>
        /// Processes specifically the <!--{}--> comments which contain code blocks
        /// to be extracted as functions.
        /// </summary>
        private static string ProcessFunctionDefinitionsOnce(string input, out int found)
        {
            var output = input;
            var matches = FullFunctionRegex.Matches(output);
            var formattedFuncs = new List<string>();
            foreach (var match in matches.Cast<Match>().Reverse())
            {
                var func = match.Groups["func"].Value;
                var args = match.Groups["args"].Value;
                var prototypeClass = match.Groups["prototypeClass"].Value;
                var code = match.Groups["code"].Value;

                var start = match.Captures[0].Index;
                var length = match.Captures[0].Length;

                // Remove function code block.
                output = output.Remove(start, length);

                // Format function code into a function definition.
                // If prototype class is defined, format as a prototype function definition.
                formattedFuncs.Add(!string.IsNullOrEmpty(prototypeClass)
                                       ? string.Format(FunctionPrototypeDefinitionFormat, func, args, code, prototypeClass)
                                       : string.Format(FunctionDefinitionFormat, func, args, code));
            }

            found = formattedFuncs.Count;

            // Insert concatenated function definitions.
            var replacementIndex = output.IndexOf(FunctionPlaceholder);
            if (replacementIndex == -1)
            {
                output = string.Concat(string.Join(Environment.NewLine + Environment.NewLine, formattedFuncs) + Environment.NewLine + Environment.NewLine, output);
            }
            else
            {
                //output = output.Remove(replacementIndex, FunctionPlaceholder.Length);
                output = output.Insert(replacementIndex, string.Join(Environment.NewLine + Environment.NewLine, formattedFuncs.Reverse<string>()) + Environment.NewLine + Environment.NewLine);
            }

            return output;
        }

        /// <summary>
        /// Process data attributes and comments.
        /// </summary>
        private static string ProcessHtml(string input)
        {
            // Parse html.
            // This option forces closing of self-closing tags such as <br /> instead of <br>.
            var htmlDocument = new HtmlDocument
            {
                OptionWriteEmptyNodes = true
            };
            htmlDocument.LoadHtml(input);

            // Pull out all relevant nodes.
            // Should be able to select nodes with particular attributes by xpath, but the xpath implementation doesn't seem to support attributes.
            var allNodes = htmlDocument.DocumentNode.SelectNodes("//*");
            if (allNodes == null)
            {
                // Not properly formatted html document.
                return input;
            }

            var commentNodes = htmlDocument.DocumentNode.SelectNodes("//comment()");

            // Extract encode function
            var encodeFunction = default(string);
            if (commentNodes != null)
            {
                foreach (var commentNode in commentNodes)
                {
                    var encodeMatch = Regex.Match(commentNode.InnerText, @"^<!--@encode\s+(?<func>[^\s]+)\s*@-->$");
                    if (encodeMatch.Success)
                    {
                        encodeFunction = encodeMatch.Groups["func"].Value;
                        commentNode.Remove();
                    }
                }
            }

            // Process markup with action attributes.
            var actionAttributes = allNodes.SelectMany(i => i.Attributes).Where(i => i.Name.Equals("data-action")).ToArray();
            foreach (var attribute in actionAttributes)
            {
                var ownerNode = attribute.OwnerNode;
                switch (attribute.Value)
                {
                    // Remove node.
                    case "remove":
                        ownerNode.Remove();
                        break;

                    // Remove all children which aren't comments.
                    case "empty":
                        foreach (var child in ownerNode.ChildNodes.Where(i => !(i is HtmlCommentNode)).ToArray())
                        {
                            child.Remove();
                        }

                        attribute.Remove();
                        break;
                }
            }

            // Transform code attributes.
            var attrAttributes = allNodes.SelectMany(i => i.Attributes).Where(i => i.Name.StartsWith("data-attr-")).ToArray();
            foreach (var attribute in attrAttributes)
            {
                var attributeName = Regex.Replace(attribute.Name, "^data-attr-", string.Empty);
                var ownerNode = attribute.OwnerNode;
                ownerNode.Attributes.Remove(attribute.Name);
                ownerNode.Attributes.Remove(attributeName);

                // TODO: I don't think this works if more than one value declaration.
                var valueMatch = Regex.Match(attribute.Value, @"\[%=\s*(?<value>.*?)\s*%(?<flags>[r]*)\]");
                if (valueMatch.Success)
                {
                    // Extract flags.
                    var flags = valueMatch.Groups["flags"].Value.Distinct().ToArray();

                    // Format code block.
                    var code = valueMatch.Groups["value"].Value;
                    if (!string.IsNullOrEmpty(code))
                    {
                        // Encode if encode function specified and no raw flag.
                        if (!string.IsNullOrEmpty(encodeFunction) && !flags.Contains('r'))
                        {
                            code = string.Format("{0}({1})", encodeFunction, code);
                        }

                        ownerNode.Attributes.Add(attributeName, string.Format("<%= {0} %>", HttpUtility.HtmlDecode(code)));
                    }
                }
                else
                {
                    ownerNode.Attributes.Add(attributeName, attribute.Value);
                }
            }

            // Process comments.
            if (commentNodes != null)
            {
                foreach (var commentNode in commentNodes)
                {
                    var commentText = Regex.Replace(commentNode.InnerText, "^<!--(.*)-->$", "$1", RegexOptions.Singleline);

                    // Remove comment node.
                    var removeMatch = Regex.Match(commentText, "^x(?<value>.*?)x$", RegexOptions.Singleline);
                    if (removeMatch.Success)
                    {
                        commentNode.Remove();
                        continue;
                    }

                    // Substitute comment node for the raw value.
                    var markupMatch = Regex.Match(commentText, "^=(?<value>.*?)=$", RegexOptions.Singleline);
                    if (markupMatch.Success)
                    {
                        var textNode = htmlDocument.CreateTextNode(markupMatch.Groups["value"].Value);
                        commentNode.ParentNode.InsertAfter(textNode, commentNode);
                        commentNode.Remove();
                        continue;
                    }

                    // Substitute comment node for value embedded in value block.
                    var valueMatch = Regex.Match(commentText, @"^\/=\s*(?<value>.*?)\s*\/(?<flags>[tr]*)$");
                    if (valueMatch.Success)
                    {
                        // Extract flags.
                        var flags = valueMatch.Groups["flags"].Value.Distinct().ToArray();

                        // Remove sibling text node.
                        if (flags.Contains('t'))
                        {
                            var next = commentNode.NextSibling;
                            if (next is HtmlTextNode)
                            {
                                next.Remove();
                            }
                        }

                        // Replace comment with code block.
                        var code = valueMatch.Groups["value"].Value;
                        if (!string.IsNullOrEmpty(code))
                        {
                            // Encode if encode function specified and no raw flag.
                            if (!string.IsNullOrEmpty(encodeFunction) && !flags.Contains('r'))
                            {
                                code = string.Format("{0}({1})", encodeFunction, code);
                            }

                            var textNode = htmlDocument.CreateTextNode(string.Format("<%= {0} %>", code));
                            commentNode.ParentNode.InsertAfter(textNode, commentNode);
                        }

                        commentNode.Remove();
                        continue;
                    }

                    // Substitute comment node for value embedded in code block.
                    var codeMatch = Regex.Match(commentText, @"^\/(?<value>.*?)\/(?<flags>[st]*)$", RegexOptions.Singleline);
                    if (codeMatch.Success)
                    {
                        // Extract flags.
                        var flags = codeMatch.Groups["flags"].Value.Distinct().ToArray();

                        // Remove sibling text node.
                        if (flags.Contains('t'))
                        {
                            var next = commentNode.NextSibling;
                            if (next is HtmlTextNode)
                            {
                                next.Remove();
                            }
                        }

                        // Replace comment with code block.
                        var code = codeMatch.Groups["value"].Value;
                        if (!string.IsNullOrEmpty(code))
                        {
                            var textNode = htmlDocument.CreateTextNode(string.Format("<%{0}%>{1}", code, flags.Contains('s') ? " " : null));
                            commentNode.ParentNode.InsertAfter(textNode, commentNode);
                        }

                        commentNode.Remove();
                        continue;
                    }
                }
            }
            
            // Generate output as string.
            var outputWriter = new StringWriter();
            htmlDocument.Save(outputWriter);
            return outputWriter.ToString();
        }

        /// <summary>
        /// Processes specifically the <!--@include-xxx @--> comments which contain include directives
        /// </summary>
        private static string ProcessIncludes(string input, string workingDirectory, string when)
        {
            var output = input;
            var matches = IncludeWhenRegex.Matches(output);
            foreach (var match in matches.Cast<Match>().Reverse())
            {
                var itemWhen = match.Groups["when"].Value;
                if (itemWhen != when)
                {
                    continue;
                }

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

                    // If treat as html, extract and include only the content of the body tag.
                    var includeContent = flags.Contains('h')
                                             ? GetHtmlBody(fileContent)
                                             : fileContent;
                    if (includeContent == null)
                    {
                        Log.Warn($"No includable content in file: {fullPath}.");
                    }
                    else
                    {
                        includeContent = ProcessIncludes(includeContent, Path.GetDirectoryName(fullPath));
                    }

                    output = output.Insert(start, includeContent ?? string.Empty);
                }
                else
                {
                    Log.Warn($"Cannot include file in template, as it does not exist: {fullPath}.");
                }
            }

            return output;
        }

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

        private static string GetHtmlBody(string input)
        {
            // This option forces closing of self-closing tags such as <br /> instead of <br>.
            var htmlDocument = new HtmlDocument
            {
                OptionWriteEmptyNodes = true
            };

            htmlDocument.LoadHtml(input);

            // Return the content of either the data-container, or the body, or the whole file content.
            var containerNode = htmlDocument.DocumentNode.SelectSingleNode("//*[@data-container]")
                             ?? htmlDocument.DocumentNode.SelectSingleNode("html/body");
            return containerNode != null
                ? containerNode.InnerHtml.Trim()
                : null;
        }

        #endregion
    }
}
