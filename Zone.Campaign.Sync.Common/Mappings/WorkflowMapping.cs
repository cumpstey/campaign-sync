using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using log4net;
using Zone.Campaign;
using Zone.Campaign.Templates;
using Zone.Campaign.Templates.Model;
using Zone.Campaign.Templates.Services;
using Zone.Campaign.WebServices.Model;
using Zone.Campaign.WebServices.Model.Abstract;
using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.Mappings
{
    /// <summary>
    /// Contains helper methods for mapping between a .NET class and information formatted for Campaign to understand.
    /// </summary>
    public class WorkflowMapping : FolderItemMapping<Workflow>
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(WorkflowMapping));

        private readonly string[] _queryFields = { "@internalName", "@label", "data", "folder/@name" };

        private readonly WorkflowTransformer _transformer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="WorkflowMapping"/>
        /// </summary>
        /// <param name="queryService">Query service</param>
        /// <param name="transformer">Template transformer</param>
        public WorkflowMapping(IQueryService queryService, WorkflowTransformer transformer)
            : base(queryService)
        {
            _transformer = transformer;
        }

        #endregion

        #region Properties

        /// <summary>
        /// List of field names which should be requested when querying Campaign.
        /// </summary>
        public override IEnumerable<string> QueryFields { get { return _queryFields; } }

        /// <summary>
        /// List of the attributes on the root element which should be persisted to the local file on download.
        /// </summary>
        public virtual IEnumerable<string> AttributesToKeep { get { return new string[0]; } }

        #endregion

        #region Methods

        /// <summary>
        /// Map the information parsed from a file into a class which can be sent to Campaign to be saved.
        /// </summary>
        /// <param name="requestHandler">Request handler, which can be used if further information from Campaign is required for the mapping.</param>
        /// <param name="template">Class containing file content and metadata.</param>
        /// <returns>Class containing information which can be sent to Campaign</returns>
        public override IPersistable GetPersistableItem(IRequestHandler requestHandler, Template template)
        {
            var folderId = default(int?);
            if (template.Metadata.AdditionalProperties.ContainsKey("Folder"))
            {
                folderId = GetFolderId(requestHandler, template.Metadata.AdditionalProperties["Folder"]);
            }

            var doc = new XmlDocument();
            try
            {
                doc.LoadXml(template.Code);
            }
            catch (Exception ex)
            {
                Log.Error($"Error loading xml for {template.Metadata.Name}.", ex);
                return null;
            }

            // Substitute delivery internal name for id in each delivery activity.
            var deliveryActions = doc.SelectNodes("/workflow/activities/delivery");
            if (deliveryActions != null)
            {
                foreach (XmlElement deliveryAction in deliveryActions)
                {
                    var deliveryInternalNameAttr = deliveryAction.Attributes["scenario-internalName"];
                    if (deliveryInternalNameAttr == null)
                    {
                        continue;
                    }

                    var deliveryInternalName = deliveryInternalNameAttr.Value;

                    var deliveryId = GetDeliveryId(requestHandler, deliveryInternalName);
                    if (deliveryId == null)
                    {
                        continue;
                    }

                    deliveryAction.Attributes.Remove(deliveryInternalNameAttr);
                    deliveryAction.AppendAttribute("scenario-id", deliveryId.ToString());
                }
            }

            return new Workflow
            {
                Name = template.Metadata.Name,
                Label = template.Metadata.Label,
                FolderId = folderId,
                RawXml = doc.OuterXml,
            };
        }

        /// <summary>
        /// Map the information sent back by Campaign into a format which can be saved as a file to disk.
        /// </summary>
        /// <param name="requestHandler">Request handler, which can be used if further information from Campaign is required for the mapping.</param>
        /// <param name="rawQueryResponse">Raw response from Campaign.</param>
        /// <returns>Class containing file content and metadata</returns>
        public override Template ParseQueryResponse(IRequestHandler requestHandler, string rawQueryResponse)
        {
            var doc = new XmlDocument();
            doc.LoadXml(rawQueryResponse);

            var metadata = new TemplateMetadata
            {
                Schema = InternalName.Parse(Schema),
                Name = new InternalName(null, doc.DocumentElement.Attributes["internalName"].InnerText),
                Label = doc.DocumentElement.Attributes["label"].InnerText,
            };

            var folderInternalName = doc.DocumentElement.SelectSingleNode("folder").Attributes["name"].InnerText;
            metadata.AdditionalProperties.Add("Folder", folderInternalName);

            doc.DocumentElement.RemoveAllAttributesExcept(AttributesToKeep);
            doc.DocumentElement.RemoveChild("createdBy");
            doc.DocumentElement.RemoveChild("modifiedBy");
            
            // Substitute delivery internal name for id in each delivery activity.
            var deliveryActions = doc.DocumentElement.SelectNodes("activities/delivery");
            if (deliveryActions != null)
            {
                foreach (XmlElement deliveryAction in deliveryActions)
                {
                    var deliveryIdAttr = deliveryAction.Attributes["scenario-id"];
                    if (deliveryIdAttr == null)
                    {
                        continue;
                    }

                    int deliveryId;
                    if (!int.TryParse(deliveryIdAttr.Value, out deliveryId))
                    {
                        continue;
                    }

                    var deliveryInternalName = GetDeliveryInternalName(requestHandler, deliveryId);
                    if (deliveryInternalName == null)
                    {
                        continue;
                    }

                    deliveryAction.Attributes.Remove(deliveryIdAttr);
                    deliveryAction.AppendAttribute("scenario-internalName", deliveryInternalName);
                }
            }

            var rawCode = doc.OuterXml;
            return new Template
            {
                Code = rawCode,
                Metadata = metadata,
                FileExtension = FileTypes.Xml,
            };
        }

        /// <summary>
        /// Retrieves the appropriate template transformer for a workflow.
        /// </summary>
        /// <param name="fileExtension">Extension of the file being processed</param>
        /// <returns>An instance of a template transformer</returns>
        public override ITemplateTransformer GetTransformer(string fileExtension)
        {
            return _transformer;
        }

        #endregion

        #region Helpers
        
        private int? GetDeliveryId(IRequestHandler requestHandler, string internalName)
        {
            var queryResponse = QueryService.ExecuteQuery(requestHandler, "nms:delivery", new[] { "@id" }, new[] { $"@internalName = '{internalName}'" });
            if (!queryResponse.Success)
            {
                Log.Error($"Failed to retrieve internal name of delivery {internalName}: {queryResponse.Message}", queryResponse.Exception);
                return null;
            }

            if (queryResponse.Data.Count() != 1)
            {
                Log.Warn($"Delivery {internalName} referenced but not found.");
                return null;
            }

            var doc = new XmlDocument();
            try
            {
                doc.LoadXml(queryResponse.Data.First());
                var id = int.Parse(doc.DocumentElement.Attributes["id"].Value);
                return id;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to parse id from delivery xml.", ex);
                return null;
            }
        }

        private string GetDeliveryInternalName(IRequestHandler requestHandler, int id)
        {
            var queryResponse = QueryService.ExecuteQuery(requestHandler, "nms:delivery", new[] { "@internalName" }, new[] { $"@id = {id}" });
            if (!queryResponse.Success)
            {
                Log.Error($"Failed to retrieve internal name of delivery {id}: {queryResponse.Message}", queryResponse.Exception);
                return null;
            }

            if (queryResponse.Data.Count() != 1)
            {
                Log.Warn($"Delivery {id} referenced but not found.");
                return null;
            }

            var doc = new XmlDocument();
            try
            {
                doc.LoadXml(queryResponse.Data.First());
                var internalName = doc.DocumentElement.Attributes["internalName"].Value;
                return internalName;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to parse internal name from delivery xml.", ex);
                return null;
            }
        }

        #endregion
    }
}
