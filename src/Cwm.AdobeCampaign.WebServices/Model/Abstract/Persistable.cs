using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Cwm.AdobeCampaign.WebServices.Model.Abstract
{
    /// <summary>
    /// Base class for classes representing objects which are persistable in Adobe Campaign.
    /// </summary>
    public abstract class Persistable : IPersistable
    {
        #region Methods

        /// <summary>
        /// Formats the data into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <returns>Xml element containing all the properties to update</returns>
        public abstract XElement GetXmlForPersist();

        #endregion

        #region Helpers

        /// <summary>
        /// Formats the data into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <param name="key">The comma-separated xpath(s) of the field(s) which should be used as the key in a persist request.</param>
        /// <returns>Xml element containing all the properties to update</returns>
        /// <exception cref="InvalidOperationException">Thrown if the class doesn't have a <see cref="SchemaAttribute" /> attribute.</exception>
        protected XElement GetBaseXmlForPersist(string key)
        {
            var schemaAttribute = GetType().GetCustomAttribute<SchemaAttribute>(false);
            if (schemaAttribute == null)
            {
                throw new InvalidOperationException($"Class {GetType().FullName} must have a {typeof(SchemaAttribute).Name} attribute to be used as a persistable entity.");
            }

            var schema = schemaAttribute.Name;
            var elementName = schema.Split(':').Last();

            var element = new XElement(elementName,
                new XAttribute("_operation", "insertOrUpdate"),
                new XAttribute("xtkschema", schema));

            if (!string.IsNullOrWhiteSpace(key))
            {
                element.Add(new XAttribute("_key", key));
            }

            return element;
        }

        #endregion
    }
}
