using System;

namespace Cwm.AdobeCampaign.WebServices.Services.Responses
{
    /// <summary>
    /// Class to hold information about the response to a request made to Campaign.
    /// </summary>
    /// <typeparam name="T">Type of the data returned by Campaign</typeparam>
    public class Response<T> : Response
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class
        /// with a status, and optional message and exception.
        /// </summary>
        /// <param name="status">Status of the response</param>
        /// <param name="message">Message about the response</param>
        /// <param name="exception">Exception thrown in making the request</param>
        public Response(ResponseStatus status, string message = null, Exception exception = null)
            : base(status, message, exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class
        /// with a status, and optional message and exception.
        /// </summary>
        /// <param name="status">Status of the response</param>
        /// <param name="data">Data returned by Campaign</param>
        public Response(ResponseStatus status, T data)
            : base(status)
        {
            Data = data;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Data returned by Campaign.
        /// </summary>
        public T Data { get; private set; }

        #endregion
    }
}
