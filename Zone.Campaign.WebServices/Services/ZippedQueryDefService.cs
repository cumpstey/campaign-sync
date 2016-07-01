using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Zone.Campaign.WebServices.Security;
using Zone.Campaign.WebServices.Services.Abstract;
using Zone.Campaign.WebServices.Services.Responses;
using log4net;

namespace Zone.Campaign.WebServices.Services
{
    public class ZippedQueryDefService : ZippedService, IQueryService
    {
        #region Fields

        private const string ServiceNamespace = "zon:queryDef";

        private static readonly ILog Log = LogManager.GetLogger(typeof(ZippedQueryDefService));

        #endregion

        #region Methods

        public Response<IEnumerable<string>>  ExecuteQuery(Uri rootUri, Tokens tokens, string schema, IEnumerable<string> fields, IEnumerable<string> conditions)
        {
            const string serviceName = "ExecuteQueryZip";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest(serviceName, serviceNs, tokens);
            var requestDoc = serviceElement.OwnerDocument;

            // Build request for this service.
            ////var entityElement = serviceElement.AppendChild("urn:entity", serviceNs);
            //var entityElement = requestDoc.CreateElement("urn:entity");
            //var queryDefElement = entityElement.AppendChild("queryDef");
            var queryDefElement = requestDoc.CreateElement("queryDef");
            queryDefElement.AppendAttribute("operation", "select");
            queryDefElement.AppendAttribute("schema", schema);
            var selectElement = queryDefElement.AppendChild("select");
            var whereElement = queryDefElement.AppendChild("where");

            foreach (var field in fields)
            {
                var nodeElement = selectElement.AppendChild("node");
                nodeElement.AppendAttribute("expr", string.Format("[{0}]", field));
            }

            if (conditions != null)
            {
                foreach (var condition in conditions)
                {
                    var conditionElement = whereElement.AppendChild("condition");
                    conditionElement.AppendAttribute("expr", condition);
                }
            }

            var encodedQuery = ZipAndEncodeQuery(queryDefElement, serviceName);
            serviceElement.AppendChildWithValue("urn:input", encodedQuery);

            // Execute request and get response from server.
            var response = ExecuteRequest(rootUri, tokens, serviceName, ServiceNamespace, requestDoc);
            if (!response.Success)
            {
                return new Response<IEnumerable<string>>(response.Status, response.Message, response.Exception);
            }

            Log.DebugFormat("Response to {0} {1} received: {2}", serviceName, schema, response.Status);

            // Parse response to extract data as strings, which can be deserialized elsewhere.
            // This should always be there - any unsuccessful response should be caught above.
            var collectionNode = SelectSingleNode(response.Data, "urn:pdomOutput/urn:*", serviceNs);
            var itemNodes = collectionNode.ChildNodes.Cast<XmlNode>();

            return new Response<IEnumerable<string>>(ResponseStatus.Success, itemNodes.Select(i => i.OuterXml).ToArray());
        }

        #endregion
    }
}
