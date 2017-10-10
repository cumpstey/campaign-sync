using System;
using System.Xml.Linq;

namespace Cwm.AdobeCampaign.WebServices.Model.Abstract
{
    /// <summary>
    /// Class representing an entity (xtk:entity) - a base class within Campaign for xml-based classes.
    /// </summary>
    public abstract class Entity : Persistable
    {
        #region Properties

        /// <summary>
        /// Internal name, combining namespace and name.
        /// </summary>
        public InternalName Name { get; set; }

        /// <summary>
        /// Label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The raw xml of the code file as a string.
        /// </summary>
        public string RawXml { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the data into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <returns>Xml element containing all the properties to update</returns>
        /// <exception cref="InvalidOperationException">Thrown when properties have values which are invalid for persisting.</exception>
        public override XElement GetXmlForPersist()
        {
            if (Name == null)
            {
                throw new InvalidOperationException($"Can't persist entity when the {nameof(Name)} has not been set.");
            }

            // Official documentation (https://docs.campaign.adobe.com/doc/AC6.1/en/CFG_API_Data_oriented_APIs.html)
            // suggests that the _key attribute is necessary. In fact, not only is it not necessary, but it under
            // some circumstances breaks the schema: if the schema represents _only_ soap methods, not a database
            // table, then including the _key attribute in a persist request will mean that the schema is left in
            // a state which causes the database update process to fail.
            ////var element = GetBaseXmlForPersist(ownerDocument, "@namespace, @name");

            var element = GetBaseXmlForPersist(null);
            element.Add(new XAttribute("namespace", Name.Namespace));
            element.Add(new XAttribute("name", Name.Name));

            if (Label != null)
            {
                element.Add(new XAttribute("label", Label));
            }

            if (RawXml != null)
            {
                XDocument contentDoc;
                try
                {
                    contentDoc = XDocument.Parse(RawXml);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Can't persist entity when {nameof(RawXml)} is not valid xml.", ex);
                }

                foreach (var attribute in contentDoc.Root.Attributes())
                {
                    element.Add(new XAttribute(attribute.Name, attribute.Value));
                }

                foreach (var child in contentDoc.Root.Elements())
                {
                    element.Add(child);
                }
            }

            return element;
        }

        #endregion
    }
}
