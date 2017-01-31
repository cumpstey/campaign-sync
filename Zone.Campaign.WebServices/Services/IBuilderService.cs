using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Contains schema builder functions.
    /// </summary>
    public interface IBuilderService
    {
        #region Methods

        /// <summary>
        /// Trigger a build of the schema from the srcSchema
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="schemaName">Name of the schema to build</param>
        /// <returns>Response</returns>
        Response BuildSchema(IRequestHandler requestHandler, InternalName schemaName);

        #endregion
    }
}
