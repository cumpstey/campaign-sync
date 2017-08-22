using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    /// <summary>
    /// Class representing a publication model (ncm:publishing).
    /// </summary>
    [Schema(EntitySchema)]
    public class NavigationHierarchy : Entity
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string EntitySchema = "xtk:navtree";

        #endregion
    }
}
