using System.Xml;
using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    [Schema(Schema)]
    public class SrcSchema : Entity
    {
        #region Fields

        public const string Schema = "xtk:srcSchema";

        #endregion

        #region Properties

        //public bool? Library { get; set; }

        #endregion

        #region Methods

        //public override XmlElement GetXmlForPersist(XmlDocument ownerDocument)
        //{
        //    var element = GetXmlForPersist(ownerDocument);

        //    if (Library != null)
        //    {
        //        element.AppendAttribute("library", Library.ToString().ToLower());
        //    }

        //    return element;
        //}

        #endregion
    }
}
