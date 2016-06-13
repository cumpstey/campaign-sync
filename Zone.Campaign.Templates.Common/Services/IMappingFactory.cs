using Zone.Campaign.Templates.Common.Mappings;

namespace Zone.Campaign.Templates.Services
{
    public interface IMappingFactory
    {
        IMapping GetMapping(string schema);
    }
}
