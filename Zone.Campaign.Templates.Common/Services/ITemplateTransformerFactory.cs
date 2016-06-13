namespace Zone.Campaign.Templates.Services
{
    public interface ITemplateTransformerFactory
    {
        ITemplateTransformer GetTransformer(string fileExtension);
    }
}
