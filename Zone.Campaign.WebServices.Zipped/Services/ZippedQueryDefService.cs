using System.Collections.Generic;
using System.Linq;
using System.Xml;
using log4net;
using Zone.Campaign.WebServices.Services.Abstract;
using Zone.Campaign.WebServices.Services.Responses;

namespace Zone.Campaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the zon:queryDef SOAP services, which accept zipped and base64 encoded SOAP requests.
    /// </summary>
    public class ZippedQueryDefService : ZippedService, IQueryService
    {
        #region Fields

        private const string ServiceNamespace = "zon:queryDef";

        private static readonly ILog Log = LogManager.GetLogger(typeof(ZippedQueryDefService));

        #endregion

        #region Methods

        /// <summary>
        /// Query the data based on a set of conditions.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="schema">Schema of the data to query</param>
        /// <param name="fields">Fields to return</param>
        /// <param name="conditions">Conditions</param>
        /// <returns>Response containing collection of matching items</returns>
        public Response<IEnumerable<string>> ExecuteQuery(IRequestHandler requestHandler, string schema, IEnumerable<string> fields, IEnumerable<string> conditions)
        {
            const string serviceName = "ExecuteQueryZip";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest(serviceName, serviceNs);
            var requestDoc = serviceElement.OwnerDocument;

            // Build request for this service.
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
            var response = requestHandler.ExecuteRequest(new ServiceName(ServiceNamespace, serviceName), requestDoc);
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
