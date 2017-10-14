using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Cwm.AdobeCampaign.WebServices.Services;
using Microsoft.Extensions.Logging;

namespace Cwm.AdobeCampaign.Sync.Mappings
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

        private readonly ILogger _logger;

        private readonly IQueryService _queryService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="FolderItemMapping{T}"/>
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="queryService">Query service</param>
        public FolderItemMapping(ILoggerFactory loggerFactory, IQueryService queryService)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<FolderItemMapping<T>>();
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
        protected async Task<int?> GetFolderIdAsync(IRequestHandler requestHandler, string internalName)
        {
            var queryResponse = await _queryService.QueryAsync(requestHandler, "xtk:folder", new[] { "@id" }, new[] { $"@name = '{internalName}'" });
            if (!queryResponse.Success)
            {
                _logger.LogError(queryResponse.Exception, $"Failed to retrieve internal name of folder {internalName}: {queryResponse.Message}");
                return null;
            }

            if (queryResponse.Data.Count() != 1)
            {
                _logger.LogWarning($"Folder {internalName} referenced but not found.");
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
                _logger.LogError(ex, "Failed to parse id from folder xml.");
                return null;
            }
        }

        #endregion
    }
}
