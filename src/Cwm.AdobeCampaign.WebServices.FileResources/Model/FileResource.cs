using System;
using System.Xml;
using System.Xml.Linq;
using Cwm.AdobeCampaign.WebServices.Model.Abstract;

namespace Cwm.AdobeCampaign.WebServices.Model
{
    /// <summary>
    /// Class representing a file resource (xtk:fileRes).
    /// </summary>
    [Schema(EntitySchema)]
    public class FileResource : Persistable, IPersistable
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string EntitySchema = "xtk:FileRes";

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
        /// Alt text for the image.
        /// </summary>
        public string Alt { get; set; }

        /// <summary>
        /// Width of the image.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Height of the image.
        /// </summary>
        public int? Height { get; set; }

        #endregion

        #region Methods
        
        /// <summary>
        /// Formats the dataa into appropriate xml for sending in a persist request to Campaign.
        /// </summary>
        /// <returns>Xml element containing all the properties to update</returns>
        public override XElement GetXmlForPersist()
        {
            throw new NotImplementedException();
            //var element = GetBaseXmlForPersist(ownerDocument, "@internalName");
            //element.AppendAttribute("internalName", Name.Name);

            //if (!string.IsNullOrEmpty(Label))
            //{
            //    element.AppendAttribute("label", Label);
            //}

            //if (!string.IsNullOrEmpty(Alt))
            //{
            //    element.AppendAttribute("alt", Label);
            //}

            //if (Width == null)
            //{
            //    element.AppendAttribute("width", Width.ToString());
            //}

            //if (Height == null)
            //{
            //    element.AppendAttribute("height", Height.ToString());
            //}

            //return element;
        }

        #endregion
    }
}
