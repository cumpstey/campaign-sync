using System;

namespace Cwm.AdobeCampaign.Templates.Exceptions
{
    /// <summary>
    /// Exception throw when parsing of metadata fails.
    /// </summary>
    public class MultipleMetadataException : Exception
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleMetadataException"/> class.
        /// </summary>
        public MultipleMetadataException(int count)
            : base($"{count} metadata blocks found")
        {
            Count = count;
        }

        #endregion

        #region Properties

        public int Count { get; private set; }

        #endregion
    }
}
