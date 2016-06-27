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
    public class ZippedPersistService : ZippedService, IWriteService
    {
        #region Fields

        private const string ServiceNamespace = "zon:persist";

        private static readonly ILog Log = LogManager.GetLogger(typeof(PersistService));

        #endregion

        #region Methods

        public Response Write<T>(Uri rootUri, Tokens tokens, T item)
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
            var itemElement = item.GetXmlForPersist(requestDoc);
            var encodedQuery = ZipAndEncodeQuery(itemElement, serviceName);
            serviceElement.AppendChildWithValue("urn:input", encodedQuery);

            // Execute request and get response from server.
            var response = ExecuteRequest(rootUri, tokens, serviceName, ServiceNamespace, requestDoc);

            Log.DebugFormat("Response to {0} {1} received: {2}", serviceName, schema, response.Status);

            return response;
        }

        public Response WriteCollection<T>(Uri rootUri, Tokens tokens, IEnumerable<T> items)
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
            var collectionElement = serviceElement.OwnerDocument.CreateElement("collection");
            collectionElement.AppendAttribute("xtkschema", schema);

            foreach (var item in items)
            {
                var itemElement = item.GetXmlForPersist(requestDoc);
                collectionElement.AppendChild(itemElement);
            }

            var encodedQuery = ZipAndEncodeQuery(collectionElement, serviceName);
            serviceElement.AppendChildWithValue("urn:input", encodedQuery);

            // Execute request and get response from server.
            var response = ExecuteRequest(rootUri, tokens, serviceName, ServiceNamespace, requestDoc);

            Log.DebugFormat("Response to {0} {1} received: {2}", serviceName, schema, response.Status);

            return response;
        }
        
        #endregion
    }
}
