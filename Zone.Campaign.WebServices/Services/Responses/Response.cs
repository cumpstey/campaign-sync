using System;

namespace Zone.Campaign.WebServices.Services.Responses
{
    /// <summary>
    /// Class to hold information about the response to a request made to Campaign.
    /// </summary>
    public class Response
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class
        /// with a status, and optional message and exception.
        /// </summary>
        /// <param name="status">Status of the response</param>
        /// <param name="message">Message about the response</param>
        /// <param name="exception">Exception thrown in making the request - should be null if the request was successful</param>
        public Response(ResponseStatus status, string message = null, Exception exception = null)
        {
            Status = status;
            Message = message;
            Exception = exception;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Response status.
        /// </summary>
        public ResponseStatus Status { get; private set; }

        /// <summary>
        /// Message about the response.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Exception thrown in making the request - should be null if the request was successful.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Whether the request was successful.
        /// </summary>
        public bool Success
        {
            get { return Status == ResponseStatus.Success; }
        }

        #endregion
    }
}
