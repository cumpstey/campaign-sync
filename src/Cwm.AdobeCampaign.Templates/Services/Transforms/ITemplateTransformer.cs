using System.Collections.Generic;
using Cwm.AdobeCampaign.Templates.Model;

namespace Cwm.AdobeCampaign.Templates.Services.Transforms
{
    /// <summary>
    /// Provides functions to transform code before it's uploaded to Campaign.
    /// </summary>
    public interface ITemplateTransformer
    {
        #region Properties

        /// <summary>
        /// File types which this transformer should be used for.
        /// </summary>
        ////IEnumerable<string> CompatibleFileTypes { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Transforms code so that it can be uploaded to Campaign.
        /// Allows the content of local files to be different from that stored in Campaign.
        /// There is no reverse method, so file types which are transformed cannot be directly downloaded from Campaign
        /// into the format in which they are stored locally.
        /// </summary>
        /// <param name="template">Source content</param>
        /// <param name="parameters">Parameters to determine transform behaviour</param>
        /// <returns>Transformed content</returns>
        IEnumerable<Template> Transform(Template template, TransformParameters parameters);

        #endregion
    }
}
