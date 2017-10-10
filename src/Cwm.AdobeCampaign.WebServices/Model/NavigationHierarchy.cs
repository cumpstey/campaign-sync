using Cwm.AdobeCampaign.WebServices.Model.Abstract;

namespace Cwm.AdobeCampaign.WebServices.Model
{
    /// <summary>
    /// Class representing a navigation hierarchy (xtk:navtree).
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
