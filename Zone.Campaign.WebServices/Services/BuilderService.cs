using System;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Abstract;
using Zone.Campaign.WebServices.Services.Responses;
using log4net;

namespace Zone.Campaign.WebServices.Services
{
    public class BuilderService : Service, IBuilderService
    {
        #region Fields

        public const string ServiceNamespace = "xtk:builder";

        private static readonly ILog Log = LogManager.GetLogger(typeof(BuilderService));

        #endregion

        #region Methods

        /// <summary>
        /// Trigger a build of the schema from the srcSchema
        /// </summary>
        /// <param name="schamaName">Name of the schema to build</param>
        /// <returns>Security and session tokens</returns>
        public Response BuildSchema(Uri rootUri, Tokens tokens, InternalName schemaName)
        {
            const string serviceName = "BuildSchemaFromId";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest("BuildSchemaFromId", serviceNs, tokens);
            var requestDoc = serviceElement.OwnerDocument;

            // Build request for this service.
            serviceElement.AppendChildWithValue("urn:schemaId", serviceNs, schemaName.ToString());

            // Execute request and get response from server.
            var response = ExecuteRequest(rootUri, tokens, serviceName, ServiceNamespace, requestDoc);

            Log.DebugFormat("Response to {0} received: {1}", serviceName, response.Status);

            return response;
        }

        #endregion
    }
}
