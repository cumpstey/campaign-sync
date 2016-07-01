using System;
using System.Collections.Generic;
using Zone.Campaign.Sync.Mappings.Abstract;
using Zone.Campaign.WebServices.Model;

namespace Zone.Campaign.Sync.Mappings
{
    public class PublishingMapping : EntityMapping<Publishing>
    {
        #region Fields

        private readonly IEnumerable<string> _attributesToKeep = new[] { "checkStatus", "form-name", "form-namespace", "schema-name", "schema-namespace" };

        #endregion

        #region Properties

        public override IEnumerable<string> AttributesToKeep { get { return _attributesToKeep; } }

        #endregion
    }
}
