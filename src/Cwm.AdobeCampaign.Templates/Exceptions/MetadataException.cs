using System;

namespace Cwm.AdobeCampaign.Templates.Exceptions
{
    /// <summary>
    /// Exception throw when parsing of metadata fails.
    /// </summary>
    public class MetadataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataException"/> class
        /// with a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public MetadataException(Exception innerException)
            : base("Error parsing metadata", innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public MetadataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
