using System;
using System.Collections.Generic;
using Zone.Campaign.Sync.Mappings.Abstract;
using Zone.Campaign.Sync.Services;
using Zone.Campaign.Templates.Services;
using Zone.Campaign.Templates.Services.Metadata;
using Zone.Campaign.WebServices.Services;

namespace Zone.Campaign.Sync.UI.Initialization
{
    internal class Registry : StructureMap.Registry
    {
        private static Options Options;

        public static void SetOptions(Options options)
        {
            Options = options;
        }

        public Registry()
        {
            // Register services which follow convention
            Scan(scan =>
            {
                scan.AssemblyContainingType<IUploader>();
                scan.AssemblyContainingType<ITemplateTransformer>();
                scan.WithDefaultConventions();
                scan.AddAllTypesOf<IMapping>();
                scan.AddAllTypesOf<ITemplateTransformer>();
            });

            // Doesn't follow convention, so requires explicit declaration
            For<IMetadataExtractorFactory>().Use<MetadataProcessorFactory>();
            For<IMetadataInserterFactory>().Use<MetadataProcessorFactory>();

            For<IXmlMetadataExtractor>().Use<XmlMetadataProcessor>();

            For<IImageDataProvider>().Use<CsvImageDataProvider>();

            For<IAuthenticationService>().Use<SessionService>();
            For<IBuilderService>().Use<BuilderService>();
            For<IPublishingService>().Use<PublishingService>();
            For<IImageWriteService>().Use<ImagePersistService>();

            if (Options.RequestMode == RequestMode.Zip)
            {
                For<IQueryService>().Add<ZippedQueryDefService>().Named("Zip");
                For<IWriteService>().Add<ZippedPersistService>().Named("Zip");
            }
            else
            {
                For<IQueryService>().Use<QueryDefService>().Named("Default");
                For<IWriteService>().Use<PersistService>().Named("Default");
            }

            For<IAuthenticatedRequestHandler>().Use<HttpSoapRequestHandler>()
                .Ctor<Uri>("uri").Is(new Uri(Options.Server))
                .Ctor<IEnumerable<string>>("customHeaders").Is(Options.CustomHeaders);
        }
    }
}
