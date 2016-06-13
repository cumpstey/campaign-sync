using System;

namespace Zone.Campaign.Templates.Model
{
    [Serializable]
    public class TemplateMetadata
    {
        #region Properties

        public InternalName Schema { get; set; }

        public InternalName Name { get; set; }

        public string Label { get; set; }

        #endregion
    }
}
