using System;
using Zone.Campaign.WebServices.Model;

namespace Zone.Campaign.Templates.Common.Mappings
{
    public class FormMapping : EntityMapping
    {
        #region Properties

        protected override string Schema { get { return Form.Schema; } }

        public override Type MappingFor { get { return typeof(Form); } }
        
        #endregion
    }
}
