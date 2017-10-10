using Cwm.AdobeCampaign.WebServices.Model.Abstract;

namespace Cwm.AdobeCampaign.WebServices.Model
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
