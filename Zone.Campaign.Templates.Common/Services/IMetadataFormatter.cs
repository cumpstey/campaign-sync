using Zone.Campaign.Templates.Model;

namespace Zone.Campaign.Templates.Services
{
    public interface IMetadataFormatter
    {
        string Format(TemplateMetadata metadata);
    }
}
