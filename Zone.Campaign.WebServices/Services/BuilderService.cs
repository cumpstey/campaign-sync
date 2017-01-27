using System;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Abstract;
using Zone.Campaign.WebServices.Services.Responses;
using log4net;
using System.Collections.Generic;

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

        private readonly ISoapRequestHandler _requestHandler;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="BuilderService"/>
        /// </summary>
        /// <param name="requestHandler">Handler for the SOAP requests</param>
        public BuilderService(ISoapRequestHandler requestHandler)
        {
            if (requestHandler == null)
            {
                throw new ArgumentNullException(nameof(requestHandler));
            }

            _requestHandler = requestHandler;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Trigger a build of the schema from the srcSchema
        /// </summary>
        /// <param name="tokens">Authentication tokens</param>
        /// <param name="schemaName">Name of the schema to build</param>
        /// <returns>Response</returns>
        public Response BuildSchema(Tokens tokens, InternalName schemaName)
        {
            const string serviceName = "BuildSchemaFromId";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest("BuildSchemaFromId", serviceNs, tokens);
            var requestDoc = serviceElement.OwnerDocument;

            // Build request for this service.
            serviceElement.AppendChildWithValue("urn:schemaId", serviceNs, schemaName.ToString());

            // Execute request and get response from server.
            var response = _requestHandler.ExecuteRequest(tokens, ServiceNamespace, serviceName, requestDoc);

            Log.Debug($"Response to {serviceName} received: {response.Status}");

            return response;
        }

        #endregion
    }
}
