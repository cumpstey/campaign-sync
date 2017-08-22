using System;
using System.Linq;
using System.Xml;
using log4net;
using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.Mappings
{
    /// <summary>
    /// Contains helper methods for mapping between a .NET class and information formatted for Campaign to understand.
    /// </summary>
    public abstract class FolderItemMapping<T> : Mapping<T>
    {
        #region Fields

        /// <summary>
        /// Name of the additional data field which stores the folder.
        /// </summary>
        protected const string AdditionalData_Folder = "Folder";

        private static readonly ILog Log = LogManager.GetLogger(typeof(FolderItemMapping<T>));

        private readonly IQueryService _queryService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="FolderItemMapping{T}"/>
        /// </summary>
        /// <param name="queryService">Query service</param>
        public FolderItemMapping(IQueryService queryService)
        {
            _queryService = queryService;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Query service.
        /// </summary>
        protected IQueryService QueryService
        {
            get { return _queryService; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the id of a folder, given the internal name.
        /// </summary>
        /// <param name="requestHandler">Request handler</param>
        /// <param name="internalName">Internal name of the folder</param>
        /// <returns>Id of the folder</returns>
        protected int? GetFolderId(IRequestHandler requestHandler, string internalName)
        {
            var queryResponse = _queryService.ExecuteQuery(requestHandler, "xtk:folder", new[] { "@id" }, new[] { $"@name = '{internalName}'" });
            if (!queryResponse.Success)
            {
                Log.Error($"Failed to retrieve internal name of folder {internalName}: {queryResponse.Message}", queryResponse.Exception);
                return null;
            }

            if (queryResponse.Data.Count() != 1)
            {
                Log.Warn($"Folder {internalName} referenced but not found.");
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
                Log.Error("Failed to parse id from folder xml.", ex);
                return null;
            }
        }

        #endregion
    }
}
