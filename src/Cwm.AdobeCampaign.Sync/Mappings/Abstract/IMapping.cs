using System.Collections.Generic;
using Cwm.AdobeCampaign.Templates.Model;
using Cwm.AdobeCampaign.Templates.Services.Transforms;
using Cwm.AdobeCampaign.WebServices.Model.Abstract;
using Cwm.AdobeCampaign.WebServices.Services;

namespace Cwm.AdobeCampaign.Sync.Mappings.Abstract
{
    /// <summary>
    /// Contains helper methods for mapping between a .NET class and information formatted for Campaign to understand.
    /// </summary>
    public interface IMapping
    {
        #region Properties

        /// <summary>
        /// Adobe Campaign schema associated with this mapping class.
        /// </summary>
        string Schema { get; }

        /// <summary>
        /// List of field names which should be requested when querying Campaign.
        /// </summary>
        IEnumerable<string> QueryFields { get; }
        
        #endregion

        #region Methods

        /// <summary>
        /// Map the information parsed from a file into a class which can be sent to Campaign to be saved.
        /// </summary>
        /// <param name="requestHandler">Request handler, which can be used if further information from Campaign is required for the mapping.</param>
        /// <param name="template">Class containing file content and metadata.</param>
        /// <returns>Class containing information which can be sent to Campaign</returns>
        IPersistable GetPersistableItem(IRequestHandler requestHandler, Template template);

        /// <summary>
        /// Map the information sent back by Campaign into a format which can be saved as a file to disk.
        /// </summary>
        /// <param name="requestHandler">Request handler, which can be used if further information from Campaign is required for the mapping.</param>
        /// <param name="rawQueryResponse">Raw response from Campaign.</param>
        /// <returns>Class containing file content and metadata</returns>
        Template ParseQueryResponse(IRequestHandler requestHandler, string rawQueryResponse);

        ITemplateTransformer GetTransformer(string fileExtension);

        #endregion
    }
}
