using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services
{
    public interface IMetadataInserter
    {
        string InsertMetadata(Template input);
    }
}
