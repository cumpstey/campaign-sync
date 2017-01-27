using System;
using Zone.Campaign.Sync.Mappings;
using Zone.Campaign.Sync.Mappings.Abstract;
using Zone.Campaign.WebServices.Model;

namespace Zone.Campaign.Sync.Services
{
    /// <summary>
    /// Contains a function to return a mapping class for a given schema.
    /// </summary>
    public class MappingFactory : IMappingFactory
    {
        #region Methods

        /// <summary>
        /// Returns a mapping class for a given schema.
        /// </summary>
        /// <param name="schema">Schema</param>
        /// <returns>Mapping class</returns>
        public IMapping GetMapping(string schema)
        {
            // TODO: Make this better: compile a dictionary at runtime, or use an IoC container.
            switch (schema)
            {
                case Form.Schema:
                    return new FormMapping();
                case IncludeView.Schema:
                    return new IncludeViewMapping();
                case JavaScriptCode.Schema:
                    return new JavaScriptCodeMapping();
                case JavaScriptTemplate.Schema:
                    return new JavaScriptTemplateMapping();
                case Option.Schema:
                    return new OptionMapping();
                case Publishing.Schema:
                    return new PublishingMapping();
                case QueryFilter.Schema:
                    return new QueryFilterMapping();
                case SrcSchema.Schema:
                    return new SrcSchemaMapping();
                default:
                    throw new InvalidOperationException("Unrecognised schema.");
            }
        }

        #endregion
    }
}
