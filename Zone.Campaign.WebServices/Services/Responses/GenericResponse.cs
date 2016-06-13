using System;

namespace Zone.Campaign.WebServices.Services.Responses
{
    public class Response<T> : Response
    {
        #region Constructor

        public Response(ResponseStatus status, string message = null, Exception exception = null)
            : base(status, message, exception)
        {
        }
        
        public Response(ResponseStatus status, T data)
            : base(status)
        {
            Data = data;
        }

        #endregion

        #region Properties

        public T Data { get; private set; }

        #endregion
    }
}
