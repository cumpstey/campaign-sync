using System;
using System.Linq;
using System.Xml;

namespace Zone.Campaign.WebServices.Model.Abstract
{
    public abstract class Persistable
    {
        #region Methods

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
            element.AppendAttribute("_key", key);

            return element;
        }

        #endregion
    }
}
