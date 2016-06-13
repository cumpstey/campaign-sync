using System;
using Zone.Campaign.WebServices.Model;

namespace Zone.Campaign.Templates.Common.Mappings
{
    public class SrcSchemaMapping : EntityMapping
    {
        #region Properties

        protected override string Schema { get { return SrcSchema.Schema; } }

        public override Type MappingFor { get { return typeof(SrcSchema); } }
        
        #endregion
    }
}
