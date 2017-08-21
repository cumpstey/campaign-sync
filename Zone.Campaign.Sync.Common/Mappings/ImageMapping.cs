using System;
using System.Collections.Generic;
using System.Xml;
using Zone.Campaign.Templates;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;
using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.Mappings
{
    /// <summary>
    /// Contains helper methods for mapping between the <see cref="Image"/> .NET class and information formatted for Campaign to understand.
    /// </summary>
    public class ImageMapping : Mapping<Image>
    {
        #region Fields

        private readonly string[] _queryFields = { "@name", "@label", "@type", "data" };

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
            var imageType = (ImageType)Enum.Parse(typeof(ImageType), template.Metadata.AdditionalProperties["ImageType"], true);
            var image = new Image
            {
                Name = template.Metadata.Name,
                Label = template.Metadata.Label,
                ImageType = imageType,
                FileContent = template.Code,
            };
            return image;
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
                Schema = InternalName.Parse(Schema),
                Name = new InternalName(doc.DocumentElement.Attributes["namespace"].InnerText, doc.DocumentElement.Attributes["name"].InnerText),
            };

            var labelNode = doc.DocumentElement.Attributes["label"];
            if (labelNode != null)
            {
                metadata.Label = labelNode.InnerText;
            }

            var typeNode = doc.DocumentElement.Attributes["type"];
            if (typeNode != null)
            {
                metadata.AdditionalProperties["ImageType"] = typeNode.InnerText;
            }

            var dataNode = doc.DocumentElement.SelectSingleNode("data");
            var rawCode = dataNode == null
                          ? string.Empty
                          : dataNode.InnerText;
            return new Template
            {
                Code = rawCode,
                Metadata = metadata,
                FileExtension = FileTypes.PlainText,
            };
        }

        #endregion
    }
}
