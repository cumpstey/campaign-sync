using System;
using System.Collections.Generic;
using System.Xml;
using Zone.Campaign.Templates;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.Sync.Mappings
{
    public class OptionMapping : Mapping<Option>
    {
        #region Fields

        private readonly string[] _queryFields = { "@name", "@dataType", "@stringValue", "@longValue", "@doubleValue", "@timeStampValue", "memoValue" };

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
        /// <param name="template">Class containing file content and metadata</param>
        /// <returns>Class containing information which can be sent to Campaign</returns>
        public override IPersistable GetPersistableItem(Template template)
        {
            var dataType = (DataType)Enum.Parse(typeof(DataType), template.Metadata.AdditionalProperties["DataType"]);
            var option = new Option
            {
                Name = template.Metadata.Name,
                Label = template.Metadata.Label,
                DataType = dataType,
            };
            option.SetValue(template.Code);
            return option;
        }

        /// <summary>
        /// Map the information sent back by Campaign into a format which can be saved as a file to disk.
        /// </summary>
        /// <param name="rawQueryResponse">Raw response from Campaign</param>
        /// <returns>Class containing file content and metadata</returns>
        public override Template ParseQueryResponse(string rawQueryResponse)
        {
            var doc = new XmlDocument();
            doc.LoadXml(rawQueryResponse);

            var metadata = new TemplateMetadata
            {
                Schema = InternalName.Parse(Schema),
                Name = new InternalName(null, doc.DocumentElement.Attributes["name"].InnerText),
            };

            var option = new Option
            {
                DataType = (DataType)int.Parse(doc.DocumentElement.Attributes["dataType"].InnerText),
                StringValue = doc.DocumentElement.Attributes["stringValue"].InnerText,
                MemoValue = doc.DocumentElement.SelectSingleNode("memoValue").InnerText,
            };

            long longValue;
            if (long.TryParse(doc.DocumentElement.Attributes["longValue"].InnerText, out longValue))
            {
                option.LongValue = longValue;
            }

            double doubleValue;
            if (double.TryParse(doc.DocumentElement.Attributes["doubleValue"].InnerText, out doubleValue))
            {
                option.DoubleValue = doubleValue;
            }

            DateTime timeStampValue;
            if (DateTime.TryParse(doc.DocumentElement.Attributes["timeStampValue"].InnerText, out timeStampValue))
            {
                option.TimeStampValue = timeStampValue;
            }

            metadata.AdditionalProperties.Add("DataType", option.DataType.ToString());
            return new Template
            {
                Code = Convert.ToString(option.GetValue()),
                Metadata = metadata,
                FileExtension = FileTypes.Jssp,
            };
        }

        #endregion
    }
}
