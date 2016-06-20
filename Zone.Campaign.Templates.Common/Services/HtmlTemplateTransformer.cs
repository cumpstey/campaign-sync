using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace Zone.Campaign.Templates.Services
{
    public class HtmlTemplateTransformer : ITemplateTransformer
    {
        #region Fields

        private const string FunctionDefinitionFormat = @"function {0} ({1}) {{/-->{2}<!--/}} // end {0}";

        private const string FunctionPrototypeDefinitionFormat = @"{3}.prototype.{0} = function ({1}) {{/-->{2}<!--/}}; // end {0}";

        private const string FunctionPlaceholder = "/*{functions}*/";

        private static readonly Regex FullFunctionRegex = new Regex(@"<!--\{\s*(?<func>[a-z0-9_]+)\s*\((?<args>.*?)\)(\s*\[prototype:(?<prototypeClass>.*?)\])?\s*\}-->(?<code>.*?)<!--\{\s*end\s*\k<func>\s*\}-->", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex IncludeRegex = new Regex(@"<!--@include-(?<when>pre|post)\s*(?<path>.*?)\s*@(?<flags>[h]*)-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Constructor

        public HtmlTemplateTransformer()
        {
            //_metadataExtractor = new HtmlMetadataProcessor();
        }

        #endregion

        #region Methods

        public string Transform(string input, string workingDirectory)
        {
            var output = input;

            // Process pre-includes.
            output = ProcessIncludes(output, workingDirectory, "pre");

            // Process function definitions.
            output = ProcessFunctionDefinitions(output);

            // Process html.
            output = ProcessHtml(output);

            // Process post-includes.
            output = ProcessIncludes(output, workingDirectory, "post");

            return output;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Processes specifically the <!--{}--> comments which contain code blocks
        /// to be extracted as functions.
        /// </summary>
        private static string ProcessFunctionDefinitions(string input)
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

            // Insert concatenated function definitions.
            var replacementIndex = output.IndexOf(FunctionPlaceholder);
            if (replacementIndex == -1)
            {
                output = string.Concat(string.Join(Environment.NewLine + Environment.NewLine, formattedFuncs), output);
            }
            else
            {
                output = output.Remove(replacementIndex, FunctionPlaceholder.Length);
                output = output.Insert(replacementIndex, string.Join(Environment.NewLine + Environment.NewLine, formattedFuncs.Reverse<string>()));
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
        /// Processes specifically the <!--@include @--> comments which contain include directives
        /// </summary>
        private static string ProcessIncludes(string input, string workingDirectory, string when)
        {
            var output = input;
            var matches = IncludeRegex.Matches(output);
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
                var fullPath = Path.Combine(workingDirectory, path);
                if (File.Exists(fullPath))
                {
                    var fileContent = File.ReadAllText(fullPath);

                    // If treat as html, extract and include only the content of the body tag.
                    var includeContent = flags.Contains('h')
                                             ? GetHtmlBody(fileContent)
                                             : fileContent;

                    output = output.Insert(start, includeContent);
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
            var bodyNode = htmlDocument.DocumentNode.SelectSingleNode("html/body");
            return bodyNode != null
                       ? bodyNode.InnerHtml.Trim()
                       : null;
        }

        #endregion
    }
}
