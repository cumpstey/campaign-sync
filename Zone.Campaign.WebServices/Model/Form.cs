using Zone.Campaign.WebServices.Model.Abstract;

namespace Zone.Campaign.WebServices.Model
{
    /// <summary>
    /// Class representing a form (xtk:form).
    /// </summary>
    [Schema(Schema)]
    public class Form : Entity
    {
        #region Fields

        /// <summary>
        /// Schema represented by this class.
        /// </summary>
        public const string Schema = "xtk:form";

        #endregion
    }
}
