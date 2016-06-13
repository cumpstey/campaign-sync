using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services
{
    public interface IMetadataParser
    {
        TemplateMetadata Parse(string input);
    }
}
