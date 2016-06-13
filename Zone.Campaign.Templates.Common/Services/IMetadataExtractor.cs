using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services
{
    public interface IMetadataExtractor
    {
        Template ExtractMetadata(string input);
    }
}
