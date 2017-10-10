using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Cwm.AdobeCampaign.WebServices.Security;
using Cwm.AdobeCampaign.WebServices.Services.Abstract;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using Microsoft.Extensions.Logging;
//using log4net;

namespace Cwm.AdobeCampaign.WebServices.Services
{
    /// <summary>
    /// Wrapper for the xtk:queryDef SOAP services.
    /// </summary>
    public class QueryDefService : Service, IQueryService
    {
        #region Fields

        public const string ServiceNamespace = "xtk:queryDef";

        public const string ExecuteQueryServiceName = "ExecuteQuery";

        //private static readonly ILog Log = LogManager.GetLogger(typeof(QueryDefService));

        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public QueryDefService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QueryDefService>();
        }

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
            XNamespace serviceNs = string.Concat("urn:", ServiceNamespace);

            // Create common elements of SOAP request.
            var serviceElement = CreateServiceRequest(serviceName, serviceNs);
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
            var response = requestHandler.ExecuteRequest(new ServiceName(ServiceNamespace, serviceName), requestDoc);
            if (!response.Success)
            {
                return new Response<IEnumerable<string>>(response.Status, response.Message, response.Exception);
            }

            //Log.DebugFormat("Response to {0} {1} received: {2}", serviceName, schema, response.Status);

            // Parse response to extract data as strings, which can be deserialized elsewhere.
            // This should always be there - any unsuccessful response should be caught above.
            var collectionNode = SelectSingleNode(response.Data, "urn:pdomOutput/urn:*", serviceNs);
            var itemNodes = collectionNode.ChildNodes.Cast<XmlNode>();

            return new Response<IEnumerable<string>>(ResponseStatus.Success, itemNodes.Select(i => i.OuterXml.Replace($@" xmlns=""{serviceNs}""", string.Empty)).ToArray());
        }

        #endregion
    }
}
