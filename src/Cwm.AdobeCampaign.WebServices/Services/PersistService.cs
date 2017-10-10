using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cwm.AdobeCampaign.WebServices.Model;
using Cwm.AdobeCampaign.WebServices.Model.Abstract;
using Cwm.AdobeCampaign.WebServices.Services.Abstract;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using Microsoft.Extensions.Logging;

namespace Cwm.AdobeCampaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the xtk:persist SOAP services.
    /// </summary>
    public class PersistService : Service, IWriteService
    {
        #region Fields

        /// <summary>
        /// Soap namespace of the persist services.
        /// </summary>
        public const string ServiceNamespace = "xtk:persist";

        /// <summary>
        /// Soap name of the write service.
        /// </summary>
        public const string WriteServiceName = "Write";

        /// <summary>
        /// Soap name of the write collection service.
        /// </summary>
        public const string WriteCollectionServiceName = "WriteCollection";

        private readonly ILogger _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistService"/> class. 
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        public PersistService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PersistService>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create/update an item.
        /// </summary>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="item">Item to create/update</param>
        /// <returns>Response</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="requestHandler"/> or <paramref name="item"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <typeparamref name="T"/> class doesn't have a <see cref="SchemaAttribute" /> attribute.</exception>
        public async Task<Response> WriteAsync<T>(IRequestHandler requestHandler, T item)
            where T : IPersistable
        {
            if (requestHandler == null) throw new ArgumentNullException(nameof(requestHandler));
            if (item == null) throw new ArgumentNullException(nameof(item));

            XNamespace serviceNs = string.Concat("urn:", ServiceNamespace);

            // Get schema from attribute on the class
            var schemaAttribute = typeof(T).GetCustomAttribute<SchemaAttribute>(false);
            if (schemaAttribute == null)
            {
                throw new InvalidOperationException($"Class {GetType().FullName} must have a {typeof(SchemaAttribute).Name} attribute to be used as a persistable entity.");
            }

            var schema = schemaAttribute.Name;

            // Create common elements of SOAP request.
            var requestDoc = CreateServiceRequest(WriteServiceName, serviceNs);
            var serviceElement = GetServiceElement(requestDoc, WriteServiceName, serviceNs);

            // Build request for this service.
            var itemElement = item.GetXmlForPersist();
            var domElement = new XElement(serviceNs + "domDoc", itemElement);
            serviceElement.Add(domElement);

            // Execute request and get response from server.
            var response = await requestHandler.ExecuteRequestAsync(new ServiceName(ServiceNamespace, WriteServiceName), requestDoc);

            _logger.LogDebug($"Response to {WriteServiceName} {schema} received: {response.Status}");

            return response;
        }

        /// <summary>
        /// Create/update a collection of items.
        /// </summary>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="items">Items to create/update</param>
        /// <returns>Response</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="requestHandler"/> or <paramref name="items"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the <typeparamref name="T"/> class doesn't have a <see cref="SchemaAttribute" /> attribute.</exception>
        public async Task<Response> WriteCollectionAsync<T>(IRequestHandler requestHandler, IEnumerable<T> items)
            where T : IPersistable
        {
            if (requestHandler == null) throw new ArgumentNullException(nameof(requestHandler));
            if (items == null) throw new ArgumentNullException(nameof(items));

            XNamespace serviceNs = string.Concat("urn:", ServiceNamespace);

            // Get schema from attribute on the class.
            var schemaAttribute = typeof(T).GetCustomAttribute<SchemaAttribute>(false);
            if (schemaAttribute == null)
            {
                throw new InvalidOperationException($"Class {GetType().FullName} must have a {typeof(SchemaAttribute).Name} attribute to be used as a persistable entity.");
            }

            var schema = schemaAttribute.Name;

            // Create common elements of SOAP request
            var requestDoc = CreateServiceRequest(WriteServiceName, serviceNs);
            var serviceElement = GetServiceElement(requestDoc, WriteServiceName, serviceNs);

            // Build request for this service.
            var collectionElement = new XElement("collection", new XAttribute("xtkschema", schema));
            var domElement = new XElement(serviceNs + "domDoc", collectionElement);
            serviceElement.Add(domElement);

            foreach (var item in items)
            {
                var itemElement = item.GetXmlForPersist();
                collectionElement.Add(itemElement);
            }

            // Execute request and get response from server.
            var response = await requestHandler.ExecuteRequestAsync(new ServiceName(ServiceNamespace, WriteCollectionServiceName), requestDoc);

            _logger.LogDebug($"Response to {WriteCollectionServiceName} {schema} received: {response.Status}");

            return response;
        }

        #endregion
    }
}
