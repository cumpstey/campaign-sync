using Cwm.AdobeCampaign.WebServices.Services.Abstract;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
#if NETSTANDARD2_0
using Microsoft.Extensions.Logging;
#endif
using System.Xml.Linq;
using System.Threading.Tasks;
using System;

namespace Cwm.AdobeCampaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the xtk:builder SOAP services.
    /// </summary>
    public class BuilderService : Service, IBuilderService
    {
        #region Fields

        /// <summary>
        /// Soap namespace of the builder services.
        /// </summary>
        public const string ServiceNamespace = "xtk:builder";

        /// <summary>
        /// Soap name of the build navigation hierarchy service.
        /// </summary>
        public const string BuildNavigationHierarchyServiceName = "GenerateNavTree";

        /// <summary>
        /// Soap name of the build schema service.
        /// </summary>
        public const string BuildSchemaServiceName = "BuildSchemaFromId";

#if NETSTANDARD2_0
        private readonly ILogger _logger;
#endif

        #endregion

        #region Constructor

#if NETSTANDARD2_0
        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderService"/> class. 
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        public BuilderService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BuilderService>();
        }
#endif

        #endregion

        #region Methods

        /// <summary>
        /// Trigger a build of the entire navigation hierarchy.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <returns>Response</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="requestHandler"/> is null.</exception>
        public async Task<Response> BuildNavigationHierarchyAsync(IRequestHandler requestHandler)
        {
            if (requestHandler == null) throw new ArgumentNullException(nameof(requestHandler));

            XNamespace serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var requestDoc = CreateServiceRequest(BuildNavigationHierarchyServiceName, serviceNs);

            // Execute request and get response from server.
            var response = await requestHandler.ExecuteRequestAsync(new ServiceName(ServiceNamespace, BuildNavigationHierarchyServiceName), requestDoc);

#if NETSTANDARD2_0
            _logger.LogDebug($"Response to {BuildNavigationHierarchyServiceName} received: {response.Status}");
#endif

            return response;
        }

        /// <summary>
        /// Trigger a build of the schema from the srcSchema
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="schemaName">Name of the schema to build</param>
        /// <returns>Response</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="requestHandler"/> is null.</exception>
        public async Task<Response> BuildSchemaAsync(IRequestHandler requestHandler, InternalName schemaName)
        {
            if (requestHandler == null) throw new ArgumentNullException(nameof(requestHandler));

            XNamespace serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var requestDoc = CreateServiceRequest(BuildSchemaServiceName, serviceNs);
            var serviceElement = GetServiceElement(requestDoc, BuildSchemaServiceName, serviceNs);

            // Build request for this service.
            serviceElement.Add(new XElement(serviceNs + "schemaId", schemaName.ToString()));

            // Execute request and get response from server.
            var response = await requestHandler.ExecuteRequestAsync(new ServiceName(ServiceNamespace, BuildSchemaServiceName), requestDoc);

#if NETSTANDARD2_0
            _logger.LogDebug($"Response to {BuildSchemaServiceName} received: {response.Status}");
#endif

            return response;
        }

        #endregion
    }
}
