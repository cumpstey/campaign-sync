using System;
using System.Collections.Generic;
using Cwm.AdobeCampaign.Sync.Mappings.Abstract;
using Cwm.AdobeCampaign.WebServices.Model;

namespace Cwm.AdobeCampaign.Sync.Mappings
{
    /// <summary>
    /// Contains helper methods for mapping between the <see cref="Schema"/> .NET class and information formatted for Campaign to understand.
    /// </summary>
    public class SchemaMapping : EntityMapping<Schema>
    {
        #region Fields

        private readonly IEnumerable<string> _attributesToKeep = new[] { "extendedSchema", "implements", "library", "mappingType", "view" };

        #endregion

        #region Properties

        /// <summary>
        /// List of the attributes on the root element which should be persisted to the local file on download.
        /// </summary>
        public override IEnumerable<string> AttributesToKeep { get { return _attributesToKeep; } }

        #endregion
    }
}
