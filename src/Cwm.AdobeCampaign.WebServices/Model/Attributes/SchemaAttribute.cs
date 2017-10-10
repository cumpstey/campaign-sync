using System;

namespace Cwm.AdobeCampaign.WebServices.Model
{
    /// <summary>
    /// Attribute used to indicate the name of the Adobe Campaign schema represented by the decorated class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SchemaAttribute : Attribute
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaAttribute"/> class. 
        /// </summary>
        /// <param name="name">Name of the schema</param>
        public SchemaAttribute(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of the schema that the class decorated with this attribute represents.
        /// </summary>
        public string Name { get; private set; }

        #endregion
    }
}
