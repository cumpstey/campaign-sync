using System.Xml;
using Zone.Campaign.WebServices.Model.Abstract;
using Zone.Campaign;

namespace Zone.Campaign.WebServices.Model
{
    [Schema(Schema)]
    public class FileRes : Persistable, IPersistable
    {
        #region Fields

        public const string Schema = "xtk:FileRes";

        #endregion

        #region Properties

        public InternalName Name { get; set; }

        public string Label { get; set; }

        public string Alt { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        #endregion

        #region Methods

        public virtual XmlElement GetXmlForPersist(XmlDocument ownerDocument)
        {
            var element = GetBaseXmlForPersist(ownerDocument, "@internalName");
            element.AppendAttribute("internalName", Name.Name);

            if (!string.IsNullOrEmpty(Label))
            {
                element.AppendAttribute("label", Label);
            }

            if (!string.IsNullOrEmpty(Alt))
            {
                element.AppendAttribute("alt", Label);
            }

            if (Width == null)
            {
                element.AppendAttribute("width", Width.ToString());
            }

            if (Height == null)
            {
                element.AppendAttribute("height", Height.ToString());
            }

            return element;
        }

        #endregion
    }
}
