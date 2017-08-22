using System.Collections.Generic;
using System.Xml;
using Zone.Campaign.Templates;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.Templates.Services;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;
using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.Mappings
{
    /// <summary>
    /// Contains helper methods for mapping between the <see cref="JavaScriptTemplate"/> .NET class and information formatted for Campaign to understand.
    /// </summary>
    public class JavaScriptTemplateMapping : Mapping<JavaScriptTemplate>
    {
        #region Fields

        private readonly string[] _queryFields = { "@name", "@label", "@entitySchema", "code" };

        private readonly HtmlJavaScriptTemplateTransformer _htmlTransformer;

        private readonly JsspJavaScriptTemplateTransformer _jsspTransformer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="JavaScriptTemplateMapping"/>
        /// </summary>
        /// <param name="htmlTransformer">Template transformer for files with .html extension</param>
        /// <param name="jsspTransformer">Template transformer for files with .jssp extension</param>
        public JavaScriptTemplateMapping(HtmlJavaScriptTemplateTransformer htmlTransformer, JsspJavaScriptTemplateTransformer jsspTransformer)
        {
            _htmlTransformer = htmlTransformer;
            _jsspTransformer = jsspTransformer;
        }

        #endregion

        #region Properties

        /// <summary>
        /// List of field names which should be requested when querying Campaign.
        /// </summary>
        public override IEnumerable<string> QueryFields { get { return _queryFields; } }

        #endregion

        #region Methods

        /// <summary>
        /// Map the information parsed from a file into a class which can be sent to Campaign to be saved.
        /// </summary>
        /// <param name="requestHandler">Request handler, which can be used if further information from Campaign is required for the mapping.</param>
        /// <param name="template">Class containing file content and metadata.</param>
        /// <returns>Class containing information which can be sent to Campaign</returns>
        public override IPersistable GetPersistableItem(IRequestHandler requestHandler, Template template)
        {
            return new JavaScriptTemplate
            {
                Name = template.Metadata.Name,
                Label = template.Metadata.Label,
                Code = template.Code.Trim(),
            };
        }

        /// <summary>
        /// Map the information sent back by Campaign into a format which can be saved as a file to disk.
        /// </summary>
        /// <param name="requestHandler">Request handler, which can be used if further information from Campaign is required for the mapping.</param>
        /// <param name="rawQueryResponse">Raw response from Campaign.</param>
        /// <returns>Class containing file content and metadata</returns>
        public override Template ParseQueryResponse(IRequestHandler requestHandler, string rawQueryResponse)
        {
            var doc = new XmlDocument();
            doc.LoadXml(rawQueryResponse);

            var metadata = new TemplateMetadata
            {
                Schema = InternalName.Parse(JavaScriptTemplate.EntitySchema),
                Name = new InternalName(doc.DocumentElement.Attributes["namespace"].InnerText, doc.DocumentElement.Attributes["name"].InnerText),
                Label = doc.DocumentElement.Attributes["label"].InnerText,
            };

            var codeNode = doc.DocumentElement.SelectSingleNode("code");
            var rawCode = codeNode == null
                          ? string.Empty
                          : codeNode.InnerText;

            return new Template
            {
                Code = rawCode,
                Metadata = metadata,
                FileExtension = FileTypes.Jssp,
            };
        }

        /// <summary>
        /// Retrieves the appropriate template transformer for a JavaScript template,
        /// based on the provided file extension.
        /// </summary>
        /// <param name="fileExtension">Extension of the file being processed</param>
        /// <returns>An instance of a template transformer</returns>
        public override ITemplateTransformer GetTransformer(string fileExtension)
        {
            switch (fileExtension)
            {
                case FileTypes.Html:
                    return _htmlTransformer;
                case FileTypes.Jssp:
                    return _jsspTransformer;
                default:
                    return null;
            }
        }

        #endregion
    }
}
