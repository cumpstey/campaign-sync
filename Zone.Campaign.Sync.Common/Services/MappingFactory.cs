using System;
using System.Collections.Generic;
using System.Linq;
using Zone.Campaign.Sync.Mappings.Abstract;

namespace Zone.Campaign.Sync.Services
{
    /// <summary>
    /// Contains a function to return a mapping class for a given schema.
    /// </summary>
    public class MappingFactory : IMappingFactory
    {
        #region Fields

        private readonly IEnumerable<IMapping> _mappings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="MappingFactory"/>
        /// </summary>
        public MappingFactory(IEnumerable<IMapping> mappings)
        {
            _mappings = mappings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a mapping class for a given schema.
        /// </summary>
        /// <param name="schema">Schema</param>
        /// <returns>Mapping class</returns>
        public IMapping GetMapping(string schema)
        {
            var mapping = _mappings.FirstOrDefault(i => i.Schema == schema);
            if (mapping == null)
            {
                throw new InvalidOperationException("Unrecognised schema.");
            }

            return mapping;
        }

        #endregion
    }
}
