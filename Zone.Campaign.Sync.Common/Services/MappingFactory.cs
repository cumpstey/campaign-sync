using System;
using Zone.Campaign.Sync.Mappings;
using Zone.Campaign.Sync.Mappings.Abstract;
using Zone.Campaign.WebServices.Model;

namespace Zone.Campaign.Sync.Services
{
    public class MappingFactory : IMappingFactory
    {
        #region Methods

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
                case Publishing.Schema:
                    return new PublishingMapping();
                case SrcSchema.Schema:
                    return new SrcSchemaMapping();
                default:
                    throw new InvalidOperationException("Unrecognised schema.");
            }
        }

        #endregion
    }
}
