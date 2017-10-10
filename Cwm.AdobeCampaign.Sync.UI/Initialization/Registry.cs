using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Cwm.AdobeCampaign.WebServices.Services;
using Microsoft.Extensions.Logging;

namespace Cwm.AdobeCampaign.Sync.UI.Initialization
{
    /// <summary>
    /// Registration of services in the IoC container.
    /// </summary>
    internal class Registry : StructureMap.Registry
    {
        #region Fields

        private static Options Options;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Registry"/> class. 
        /// Configures the IoC container with the services used.
        /// </summary>
        public Registry()
        {
            // For explanation of the use of the singleton, see https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
            ForConcreteType<HttpClient>().Configure.Singleton().SelectConstructor(() => new HttpClient());

            // Register services which follow convention
            ////Scan(scan =>
            ////{
            ////    scan.AssemblyContainingType<IUploader>();
            ////    scan.AssemblyContainingType<ITemplateTransformer>();
            ////    scan.WithDefaultConventions();
            ////    scan.AddAllTypesOf<IMapping>();
            ////    scan.AddAllTypesOf<ITemplateTransformer>();
            ////});

            // Doesn't follow convention, so requires explicit declaration
            ////For<IMetadataExtractorFactory>().Use<MetadataProcessorFactory>();
            ////For<IMetadataInserterFactory>().Use<MetadataProcessorFactory>();

            ////For<IXmlMetadataExtractor>().Use<XmlMetadataProcessor>();

            ////For<IImageDataProvider>().Use<CsvImageDataProvider>();

            For<IAuthenticationService>().Use<SessionService>();
            ////For<IBuilderService>().Use<BuilderService>();
            ////For<IPublishingService>().Use<PublishingService>();
            ////For<IImageWriteService>().Use<ImagePersistService>();
            For<ITriggeredMessageService>().Use<TriggeredMessageService>();
            For<IWriteService>().Use<PersistService>();

            For<ILoggerFactory>().Singleton().Use(new LoggerFactory().AddConsole(LogLevel.Debug));

            ////if (Options.RequestMode == RequestMode.Zip)
            ////{
            ////    For<IQueryService>().Add<ZippedQueryDefService>().Named("Zip");
            ////    For<IWriteService>().Add<ZippedPersistService>().Named("Zip");
            ////}
            ////else
            ////{
            ////    For<IQueryService>().Use<QueryDefService>().Named("Default");
            ////    For<IWriteService>().Use<PersistService>().Named("Default");
            ////}

            For<IAuthenticatedRequestHandler>().Use<HttpSoapRequestHandler>()
                .Ctor<Uri>("uri").Is(new Uri(Options.ServerUrl))
                .Ctor<IEnumerable<string>>("customHeaders").Is(Options.CustomHeaders);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set options used during service registration.
        /// </summary>
        /// <param name="options">Options</param>
        public static void SetOptions(Options options)
        {
            Options = options;
        }

        #endregion
    }
}
