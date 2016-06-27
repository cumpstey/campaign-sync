using System;
using System.Collections.Generic;

namespace Zone.Campaign.Templates.Model
{
    [Serializable]
    public class TemplateMetadata
    {
        #region Constructor

        //public TemplateMetadata()
        //{
        //    AdditionalProperties = new Dictionary<string, string>();
        //}

        #endregion

        #region Properties

        public InternalName Schema { get; set; }

        public InternalName Name { get; set; }

        public string Label { get; set; }

        //public IDictionary<string, string> AdditionalProperties { get; private set; }

        #endregion
    }
}
