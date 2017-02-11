using System;
using System.Linq;
using System.Xml;

namespace Zone.Campaign.WebServices.Model.Abstract
{
    /// <summary>
    /// Base class for classes representing objects which are persistable in Adobe Campaign.
    /// </summary>
    public abstract class Persistable
    {
        #region Methods

        /// <summary>
        /// Formats the data into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <param name="ownerDocument">Document to create the xml element from</param>
        /// <param name="key">The comma-separated xpath(s) of the field(s) which should be used as the key in a persist request.</param>
        /// <returns>Xml element containing all the properties to update</returns>
        protected XmlElement GetBaseXmlForPersist(XmlDocument ownerDocument, string key)
        {
            var schemaAttribute = GetType().GetCustomAttributes(typeof(SchemaAttribute), true).FirstOrDefault() as SchemaAttribute;
            if (schemaAttribute == null)
            {
                throw new InvalidOperationException(string.Format("Class {0} must have a Schema attribute", GetType().FullName));
            }

            var schema = schemaAttribute.Name;
            var elementName = schema.Split(':').Last();

            var element = ownerDocument.CreateElement(elementName);
            element.AppendAttribute("_operation", "insertOrUpdate");
            element.AppendAttribute("xtkschema", schema);

            if (!string.IsNullOrWhiteSpace(key))
            {
                element.AppendAttribute("_key", key);
            }

            return element;
        }

        #endregion
    }
}
