using System;
using System.Collections.Generic;

namespace Zone.Campaign.Templates.Model
{
    /// <summary>
    /// Class representing metadata associated with some code, which is required for it to be uploaded to Campaign.
    /// </summary>
    [Serializable]
    public class TemplateMetadata
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="TemplateMetadata"/>
        /// </summary>
        public TemplateMetadata()
        {
            AdditionalProperties = new Dictionary<string, string>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Schema.
        /// </summary>
        public InternalName Schema { get; set; }

        /// <summary>
        /// Internal name.
        /// </summary>
        public InternalName Name { get; set; }

        /// <summary>
        /// Label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Any additional metadata for this specific item which should be sent to Campaign along with the code.
        /// </summary>
        public IDictionary<string, string> AdditionalProperties { get; private set; }

        #endregion
    }
}
