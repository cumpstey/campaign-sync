using System;
using System.Xml;
using Cwm.AdobeCampaign.WebServices.Model.Abstract;

namespace Cwm.AdobeCampaign.WebServices.Model
{
    /// <summary>
    /// Class representing an option (xtk:option).
    /// </summary>
    [Schema(EntitySchema)]
    public class Option : Persistable, IPersistable
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string EntitySchema = "xtk:option";

        #endregion

        #region Properties

        /// <summary>
        /// Internal name, combining namespace and name.
        /// </summary>
        public InternalName Name { get; set; }

        /// <summary>
        /// Label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Datatype of the data stored in this option.
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        /// Value of the data if stored as a string.
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// Value of the data if stored as an integer.
        /// </summary>
        public long? LongValue { get; set; }

        /// <summary>
        /// Value of the data if stored as a floating point number.
        /// </summary>
        public double? DoubleValue { get; set; }

        /// <summary>
        /// Value of the data if stored as a timestamp.
        /// </summary>
        public DateTime? TimeStampValue { get; set; }

        /// <summary>
        /// Value of the data if stored as a string containing linebreaks.
        /// </summary>
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
