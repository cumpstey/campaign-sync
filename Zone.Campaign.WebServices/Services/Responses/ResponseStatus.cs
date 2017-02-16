namespace Zone.Campaign.WebServices.Services.Responses
{
    /// <summary>
    /// Represents possible statuses of requests made to Campaign.
    /// </summary>
    public enum ResponseStatus
    {
        /// <summary>
        /// Request was successful.
        /// </summary>
        Success,

        /// <summary>
        /// Request was not successful, but the error could not be classified into any of the other possibilities.
        /// </summary>
        UnknownError,

        /// <summary>
        /// If the request was for login, it was made with incorrect credentials.
        /// For any other type of request, it was made with no/invalid/expired authentication tokens, or the authenticated user
        /// does not have permission to perform the requested action.
        /// </summary>
        Unauthorised,

        /// <summary>
        /// Could not connect to the Campaign server.
        /// </summary>
        ConnectionError,

        /// <summary>
        /// Connection to the Campaign server was successful, but it returned a 404 response. The SOAP handler url may be incorrect.
        /// </summary>
        NotFound,

        /// <summary>
        /// The request was invalid. More information may be found in the message about the potential cause.
        /// </summary>
        ProcessingError,
    }
}
