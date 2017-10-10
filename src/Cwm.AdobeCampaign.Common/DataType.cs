namespace Cwm.AdobeCampaign
{
    /// <summary>
    /// Data type. Corresponds to xtk:common:dataType enum in Campaign.
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// Single line string. Any line breaks in the value are liable to be lost.
        /// </summary>
        String = 6,

        /// <summary>
        /// Integer.
        /// </summary>
        Long = 3,

        /// <summary>
        /// Floating point number.
        /// </summary>
        Double = 5,

        /// <summary>
        /// Timestamp.
        /// </summary>
        TimeStamp = 7,

        /// <summary>
        /// Multiline string.
        /// </summary>
        Memo = 12,
    }
}
