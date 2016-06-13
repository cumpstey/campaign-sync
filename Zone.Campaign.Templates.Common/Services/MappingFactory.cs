using System;
using Zone.Campaign.Templates.Common.Mappings;
using Zone.Campaign.WebServices.Model;

namespace Zone.Campaign.Templates.Services
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
                case SrcSchema.Schema:
                    return new SrcSchemaMapping();
                default:
                    throw new InvalidOperationException("Unrecognised schema.");
            }
        }

        #endregion
    }
}
