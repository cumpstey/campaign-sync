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
    /// <summary>
    /// Wrapper for the xtk:queryDef SOAP services.
    /// </summary>
    public class QueryDefService : Service, IQueryService
    {
        #region Fields

        private const string ServiceNamespace = "xtk:queryDef";

        private static readonly ILog Log = LogManager.GetLogger(typeof(QueryDefService));

        private readonly ISoapRequestHandler _requestHandler;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="QueryDefService"/>
        /// </summary>
        /// <param name="requestHandler">Handler for the SOAP requests</param>
        public QueryDefService(ISoapRequestHandler requestHandler)
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
        /// Query the data based on a set of conditions.
        /// </summary>
        /// <param name="tokens">Authentication tokens</param>
        /// <param name="schema">Schema of the data to query</param>
        /// <param name="fields">Fields to return</param>
        /// <param name="conditions">Conditions</param>
        /// <returns>Response containing collection of matching items</returns>
        public Response<IEnumerable<string>> ExecuteQuery(Tokens tokens, string schema, IEnumerable<string> fields, IEnumerable<string> conditions)
        {
            const string serviceName = "ExecuteQuery";
            var serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest(serviceName, serviceNs, tokens);
            var requestDoc = serviceElement.OwnerDocument;

            // Build request for this service.
            var entityElement = serviceElement.AppendChild("urn:entity", serviceNs);
            var queryDefElement = entityElement.AppendChild("queryDef");
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

            // Execute request and get response from server.
            var response = _requestHandler.ExecuteRequest(tokens, ServiceNamespace, serviceName, requestDoc);
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
