using System;
using System.Collections.Generic;
using Cwm.AdobeCampaign.Sync.Mappings.Abstract;
using Cwm.AdobeCampaign.WebServices.Model;

namespace Cwm.AdobeCampaign.Sync.Mappings
{
    /// <summary>
    /// Contains helper methods for mapping between the <see cref="Publishing"/> .NET class and information formatted for Campaign to understand.
    /// </summary>
    public class PublishingMapping : EntityMapping<Publishing>
    {
        #region Fields

        private readonly IEnumerable<string> _attributesToKeep = new[] { "checkStatus", "form-name", "form-namespace", "schema-name", "schema-namespace" };

        #endregion

        #region Properties

        /// <summary>
        /// List of the attributes on the root element which should be persisted to the local file on download.
        /// </summary>
        public override IEnumerable<string> AttributesToKeep { get { return _attributesToKeep; } }

        #endregion
    }
}
