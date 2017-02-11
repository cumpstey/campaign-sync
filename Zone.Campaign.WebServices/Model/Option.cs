using System;
using System.Xml;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    /// <summary>
    /// Class representing an option (xtk:option).
    /// </summary>
    [Schema(Schema)]
    public class Option : Persistable, IPersistable
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string Schema = "xtk:option";

        #endregion

        #region Properties

        public InternalName Name { get; set; }

        public string Label { get; set; }

        public DataType DataType { get; set; }

        public string StringValue { get; set; }

        public long? LongValue { get; set; }

        public double? DoubleValue { get; set; }

        public DateTime? TimeStampValue { get; set; }

        public string MemoValue { get; set; }

        #endregion

        #region Methods

        public object GetValue()
        {
            switch (DataType)
            {
                case DataType.String:
                    return StringValue;
                case DataType.Long:
                    return LongValue;
                case DataType.Double:
                    return DoubleValue;
                case DataType.TimeStamp:
                    return TimeStampValue;
                case DataType.Memo:
                    return MemoValue;
                default:
                    return null;
            }
        }

        public void SetValue(object value)
        {
            switch (DataType)
            {
                case DataType.String:
                    StringValue = Convert.ToString(value);
                    break;
                case DataType.Long:
                    LongValue = Convert.ToInt64(value);
                    break;
                case DataType.Double:
                    DoubleValue = Convert.ToDouble(value);
                    break;
                case DataType.TimeStamp:
                    TimeStampValue = Convert.ToDateTime(value);
                    break;
                case DataType.Memo:
                    MemoValue = Convert.ToString(value);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Formats the dataa into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <param name="ownerDocument">Document to create the xml element from</param>
        /// <returns>Xml element containing all the properties to update</returns>
        public virtual XmlElement GetXmlForPersist(XmlDocument ownerDocument)
        {
            var element = GetBaseXmlForPersist(ownerDocument, "@name");
            element.AppendAttribute("name", Name.Name);

            element.AppendAttribute("dataType", ((int)DataType).ToString());
            element.AppendAttribute("stringValue", StringValue);
            element.AppendAttribute("longValue", LongValue.HasValue ? LongValue.Value.ToString() : string.Empty);
            element.AppendAttribute("doubleValue", DoubleValue.HasValue ? DoubleValue.Value.ToString() : string.Empty);
            element.AppendAttribute("timeStampValue", TimeStampValue.HasValue ? TimeStampValue.Value.ToString("yyyy-MM-dd HH:mm:ss.fffZ") : string.Empty);
            element.AppendChildWithValue("memoValue", MemoValue);

            return element;
        }

        #endregion
    }
}
