using log4net;
using Zone.Campaign.WebServices.Services.Abstract;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the xtk:builder SOAP services.
    /// </summary>
    public class BuilderService : Service, IBuilderService
    {
        #region Fields

        private const string ServiceNamespace = "xtk:builder";

        private static readonly ILog Log = LogManager.GetLogger(typeof(BuilderService));

        #endregion

        #region Methods

        /// <summary>
        /// Trigger a build of the entire navigation hierarchy.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <returns>Response</returns>
        public Response BuildNavigationHierarchy(IRequestHandler requestHandler)
        {
            const string serviceName = "GenerateNavTree";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest(serviceName, serviceNs);
            var requestDoc = serviceElement.OwnerDocument;

            // Execute request and get response from server.
            var response = requestHandler.ExecuteRequest(new ServiceName(ServiceNamespace, serviceName), requestDoc);

            Log.Debug($"Response to {serviceName} received: {response.Status}");

            return response;
        }

        /// <summary>
        /// Trigger a build of the schema from the srcSchema
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="schemaName">Name of the schema to build</param>
        /// <returns>Response</returns>
        public Response BuildSchema(IRequestHandler requestHandler, InternalName schemaName)
        {
            const string serviceName = "BuildSchemaFromId";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest(serviceName, serviceNs);
            var requestDoc = serviceElement.OwnerDocument;

            // Build request for this service.
            serviceElement.AppendChildWithValue("urn:schemaId", serviceNs, schemaName.ToString());

            // Execute request and get response from server.
            var response = requestHandler.ExecuteRequest(new ServiceName(ServiceNamespace, serviceName), requestDoc);

            Log.Debug($"Response to {serviceName} received: {response.Status}");

            return response;
        }

        #endregion
    }
}
