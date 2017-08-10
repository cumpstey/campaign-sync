using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Humanizer;
using Microsoft.Web.XmlTransform;
using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services
{
    /// <summary>
    /// Provides functions to transform JavaScript Server Pages code before it's uploaded to Campaign.
    /// </summary>
    public class WorkflowTransformer : ITemplateTransformer
    {
        #region Fields

        /// <summary>
        /// The extension used by transform files.
        /// </summary>
        public const string TransformFileExtension = ".config";

        #endregion

        #region Methods

        /// <summary>
        /// Applies transforms to a workflow definition code so that multiple versions can be uploaded to Campaign.
        /// Transforms are defined in workflowName.transformName.config files, using xdt syntax, analagous to ASP.NET config transforms.
        /// </summary>
        /// <param name="template">Source content</param>
        /// <param name="parameters">Parameters to determine transform behaviour</param>
        /// <returns>A collection of transformed templates</returns>
        public IEnumerable<Template> Transform(Template template, TransformParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (!parameters.ApplyTransforms)
            {
                return new[] { template };
            }

            var directoryName = Path.GetDirectoryName(parameters.OriginalFileName);
            var transformFilenamePattern = $"{Path.GetFileNameWithoutExtension(parameters.OriginalFileName)}.*{TransformFileExtension}";
            var transformFiles = Directory.GetFiles(directoryName, transformFilenamePattern);

            var templates = new List<Template>();
            foreach (var transformFile in transformFiles)
            {
                var transformName = Regex.Replace(Path.GetFileNameWithoutExtension(transformFile), $@"^{Path.GetFileNameWithoutExtension(parameters.OriginalFileName)}\.", string.Empty);

                var doc = new XmlDocument();
                doc.LoadXml(template.Code);

                var transformation = new XmlTransformation(transformFile);
                var s = transformation.Apply(doc);
                var transformedCode = doc.OuterXml;
                var transformedTemplate = new Template
                {
                    FileExtension = template.FileExtension,
                    Metadata = new TemplateMetadata
                    {
                        Name = new InternalName(null, $"{template.Metadata.Name}_{transformName}"),
                        Label = $"{template.Metadata.Label} ({transformName.Humanize()})",
                        Schema = template.Metadata.Schema,
                    },
                    Code = transformedCode,
                };
                foreach (var property in template.Metadata.AdditionalProperties)
                {
                    transformedTemplate.Metadata.AdditionalProperties.Add(property);
                }

                templates.Add(transformedTemplate);
            }

            return templates;
        }

        #endregion
    }
}
