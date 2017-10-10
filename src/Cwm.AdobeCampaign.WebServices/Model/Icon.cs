using System.Xml.Linq;
using Cwm.AdobeCampaign.WebServices.Model.Abstract;

namespace Cwm.AdobeCampaign.WebServices.Model
{
    /// <summary>
    /// Class representing an image (xtk:image).
    /// </summary>
    [Schema(EntitySchema)]
    public class Icon : Persistable
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string EntitySchema = "xtk:image";

        #endregion

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
        /// Base64 encoded image data.
        /// </summary>
        public string FileContent { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the dataa into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <returns>Xml element containing all the properties to update</returns>
        public override XElement GetXmlForPersist()
        {
            var element = GetBaseXmlForPersist("@namespace, @name");
            element.Add(new XAttribute("namespace", Name.Namespace));
            element.Add(new XAttribute("name", Name.Name));

            if (Label != null)
            {
                element.Add(new XAttribute("label", Label));
            }

            if (FileContent != null)
            {
                element.Add(new XElement("data", new XCData(FileContent)));
            }

            return element;
        }

        #endregion
    }
}
