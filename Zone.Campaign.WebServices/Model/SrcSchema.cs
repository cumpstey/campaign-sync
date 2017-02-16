using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    /// <summary>
    /// Class representing a schema source (xtk:srcSchema).
    /// </summary>
    [Schema(Schema)]
    public class SrcSchema : Entity
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string Schema = "xtk:srcSchema";

        #endregion
    }
}
