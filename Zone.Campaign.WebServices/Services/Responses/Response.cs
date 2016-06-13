using System;

namespace Zone.Campaign.WebServices.Services.Responses
{
    public class Response
    {
        #region Constructor

        public Response(ResponseStatus status, string message = null, Exception exception = null)
        {
            Status = status;
            Message = message;
            Exception = exception;
        }

        #endregion

        #region Properties

        public ResponseStatus Status { get; private set; }

        public string Message { get; private set; }

        public Exception Exception { get; private set; }

        public bool Success
        {
            get { return Status == ResponseStatus.Success; }
        }

        #endregion
    }
}
