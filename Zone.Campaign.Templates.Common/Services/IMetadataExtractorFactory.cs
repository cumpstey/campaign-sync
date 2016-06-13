namespace Zone.Campaign.Templates.Services
{
    public interface IMetadataExtractorFactory
    {
        IMetadataExtractor GetExtractor(string fileExtension);
    }
}
