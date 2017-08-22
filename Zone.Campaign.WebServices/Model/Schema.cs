using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    /// <summary>
    /// Class representing a schema source (xtk:srcSchema).
    /// </summary>
    [Schema(EntitySchema)]
    public class Schema : Entity
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string EntitySchema = "xtk:srcSchema";

        #endregion
    }
}
