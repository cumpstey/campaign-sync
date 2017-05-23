using System.Collections.Generic;
using System.Linq;

namespace Zone.Campaign.Templates.Services
{
    /// <summary>
    /// Contains a function to return a template transformer class for a given file type.
    /// </summary>
    public class TemplateTransformerFactory : ITemplateTransformerFactory
    {
        #region Fields

        private readonly IEnumerable<ITemplateTransformer> _transformers;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="TemplateTransformerFactory"/>
        /// </summary>
        public TemplateTransformerFactory(IEnumerable<ITemplateTransformer> transformers)
        {
            _transformers = transformers;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a template transformer class for given file extension.
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Template transformer class</returns>
        public ITemplateTransformer GetTransformer(string fileExtension)
        {
            var transformer = _transformers.FirstOrDefault(i => i.CompatibleFileTypes != null && i.CompatibleFileTypes.Contains(fileExtension));
            return transformer;
        }

        #endregion
    }
}
