using System;
using System.Collections.Generic;
using System.Linq;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Abstract;
using Zone.Campaign.WebServices.Services.Responses;
using log4net;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the xtk:persist SOAP services.
    /// </summary>
    public class PersistService : Service, IWriteService
    {
        #region Fields

        private const string ServiceNamespace = "xtk:persist";

        private static readonly ILog Log = LogManager.GetLogger(typeof(PersistService));

        private readonly ISoapRequestHandler _requestHandler;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="PersistService"/>
        /// </summary>
        /// <param name="requestHandler">Handler for the SOAP requests</param>
        public PersistService(ISoapRequestHandler requestHandler)
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
        /// Create/update an item.
        /// </summary>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <param name="tokens">Authentication tokens</param>
        /// <param name="item">Item to create/update</param>
        /// <returns>Response</returns>
        public Response Write<T>(Tokens tokens, T item)
            where T : IPersistable
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            const string serviceName = "Write";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Get schema from attribute on the class
            var schemaAttribute = item.GetType().GetCustomAttributes(typeof(SchemaAttribute), true).FirstOrDefault() as SchemaAttribute;
            if (schemaAttribute == null)
            {
                throw new InvalidOperationException(string.Format("Class {0} must have a Schema attribute", GetType().FullName));
            }

            var schema = schemaAttribute.Name;

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest(serviceName, serviceNs, tokens);
            var requestDoc = serviceElement.OwnerDocument;

            // Build request for this service.
            var domElement = serviceElement.AppendChild("urn:domDoc", serviceNs);

            var itemXml = item.GetXmlForPersist(requestDoc);
            domElement.AppendChild(itemXml);

            // Execute request and get response from server.
            var response = _requestHandler.ExecuteRequest(tokens, ServiceNamespace, serviceName, requestDoc);

            Log.Debug($"Response to {serviceName} {schema} received: {response.Status}");

            return response;
        }

        /// <summary>
        /// Create/update a collection of items.
        /// </summary>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <param name="tokens">Authentication tokens</param>
        /// <param name="items">Items to create/update</param>
        /// <returns>Response</returns>
        public Response WriteCollection<T>(Tokens tokens, IEnumerable<T> items)
            where T : IPersistable
        {
            const string serviceName = "WriteCollection";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Get schema from attribute on the class.
            var schemaAttribute = typeof(T).GetCustomAttributes(typeof(SchemaAttribute), true).FirstOrDefault() as SchemaAttribute;
            if (schemaAttribute == null)
            {
                throw new InvalidOperationException(string.Format("Class {0} must have a Schema attribute", GetType().FullName));
            }

            var schema = schemaAttribute.Name;

            // Create common elements of SOAP request
            var serviceElement = CreateServiceRequest(serviceName, serviceNs, tokens);
            var requestDoc = serviceElement.OwnerDocument;

            // Build request for this service.
            var domElement = serviceElement.AppendChild("urn:domDoc", serviceNs);
            var collectionElement = domElement.AppendChild("collection");
            collectionElement.AppendAttribute("xtkschema", schema);

            foreach (var item in items)
            {
                var itemXml = item.GetXmlForPersist(requestDoc);
                collectionElement.AppendChild(itemXml);
            }

            // Execute request and get response from server.
            var response = _requestHandler.ExecuteRequest(tokens, ServiceNamespace, serviceName, requestDoc);

            Log.Debug($"Response to {serviceName} {schema} received: {response.Status}");

            return response;
        }

        #endregion
    }
}
