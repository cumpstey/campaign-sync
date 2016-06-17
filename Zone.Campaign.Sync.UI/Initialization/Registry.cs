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
            For<IAuthenticationService>().Use<SessionService>();
            For<IQueryService>().Use<QueryDefService>();
            //For<IWriteService>().Use<PersistService>();
            For<IWriteService>().Use<ZippedPersistService>();
        }
    }
}
