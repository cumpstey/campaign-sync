using Zone.Campaign.Sync.Services;
using Zone.Campaign.Templates.Services;
using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.UI.Initialization
{
    internal class Registry : StructureMap.Registry
    {
        public Registry()
        {
            // Register services which follow convention
            Scan(scan =>
            {
                scan.AssemblyContainingType<IUploader>();
                scan.AssemblyContainingType<NullTemplateTransformer>();
                scan.WithDefaultConventions();
            });

            // Doesn't follow convention, so requires explicit declaration
            For<IMetadataExtractorFactory>().Use<MetadataProcessorFactory>();
            For<IMetadataInserterFactory>().Use<MetadataProcessorFactory>();

            For<IImageDataProvider>().Use<CsvImageDataProvider>();

            For<IAuthenticationService>().Use<SessionService>();
            For<IBuilderService>().Use<BuilderService>();
            For<IImageWriteService>().Use<ImagePersistService>();
            For<IQueryService>().Add<ZippedQueryDefService>().Named("Zip");
            For<IQueryService>().Use<QueryDefService>().Named("Default");
            For<IWriteService>().Add<ZippedPersistService>().Named("Zip");
            For<IWriteService>().Use<PersistService>().Named("Default");

            For<ISoapRequestHandler>().Use<HttpSoapRequestHandler>();
        }
    }
}
