using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    /// <summary>
    /// Class representing a form (xtk:form).
    /// </summary>
    [Schema(EntitySchema)]
    public class Form : Entity
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string EntitySchema = "xtk:form";

        #endregion
    }
}
