using System;
using System.Collections.Generic;
using Zone.Campaign.Sync.Mappings.Abstract;
using Zone.Campaign.WebServices.Model;

namespace Zone.Campaign.Sync.Mappings
{
    public class SrcSchemaMapping : EntityMapping<SrcSchema>
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
