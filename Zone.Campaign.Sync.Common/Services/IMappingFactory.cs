using Zone.Campaign.Sync.Mappings.Abstract;

namespace Zone.Campaign.Sync.Services
{
    public interface IMappingFactory
    {
        IMapping GetMapping(string schema);
    }
}
