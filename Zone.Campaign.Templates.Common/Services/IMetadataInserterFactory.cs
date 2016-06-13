namespace Zone.Campaign.Templates.Services
{
    public interface IMetadataInserterFactory
    {
        IMetadataInserter GetInserter(string fileExtension);
    }
}
