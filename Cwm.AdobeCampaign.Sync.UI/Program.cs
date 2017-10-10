using Cwm.AdobeCampaign.WebServices.Services;
using System;
using System.Threading.Tasks;
using Cwm.AdobeCampaign.WebServices.Model;
using System.Xml.Linq;
using StructureMap;
using Microsoft.Extensions.Logging;
using System.IO;
using Cwm.AdobeCampaign.Templates.Services.Metadata;

namespace Cwm.AdobeCampaign.Sync.UI
{
    class Program
    {
        #region Fields

        private static IContainer _container;

        #endregion

        static void Main(string[] args)
        {
            var options = new Options();

            // Set up IoC container
            Initialization.Registry.SetOptions(options);
            _container = Container.For<Initialization.Registry>();


            var uri = new Uri("http://nl.barratt.local:8080/nl/jsp/soaprouter.jsp");
            var requestHandler = _container.GetInstance<IAuthenticatedRequestHandler>();

            var authenticationService = _container.GetInstance<IAuthenticationService>();
            var logonResponse = authenticationService.LogonAsync(requestHandler, "zonesupport", "zoneremote@528").Result;
            if (!logonResponse.Success)
            {
                Console.Write("logon failed: " + logonResponse.Message);
                return;
            }

            requestHandler.AuthenticationTokens = logonResponse.Data;

            ExtractHtmlMetadata();
            //PushEvent(requestHandler);
            //WriteIcon(requestHandler);
            //WriteSchema(requestHandler);
            //WriteSchemas(requestHandler);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        #region Individual service methods

        private static void ExtractHtmlMetadata()
        {
            var input = File.ReadAllText(@"C:\Projects\Personal\Campaign sync\Code2\Cwm.AdobeCampaign\tests\Cwm.AdobeCampaign.Templates.Tests\TestResources\Services\Metadata\HtmlMetadataProcessor\Extract_Input.txt");

            var metadataProcessor = new MetadataProcessor();
            var htmlMetadataProcessor = new HtmlMetadataProcessor(metadataProcessor, metadataProcessor);
            var template = htmlMetadataProcessor.ExtractMetadata(input);
        }

        private static void PushEvent(IAuthenticatedRequestHandler requestHandler)
        {
            var eventDetails = new Event
            {
                EventType = "blah",
                Email = "ncumpstey@zonedigital.com",
                Origin = "web",
                WishedChannel = DeliveryType.Email,
                ExternalId = "123456",
                ContextData = new XElement("ctx", new XAttribute("attr", 1234), new XElement("elem", 3456)),
            };
            var triggeredMessageService = _container.GetInstance<ITriggeredMessageService>();
            var response = triggeredMessageService.PushRealTimeEventAsync(requestHandler, eventDetails).Result;
        }

        private static void WriteIcon(IAuthenticatedRequestHandler requestHandler)
        {
            var icon = new Icon
            {
                Name = new InternalName("cwm", "test.png"),
                Label = "Cwm test icon",
                FileContent = Convert.ToBase64String(File.ReadAllBytes(@"C:\temp\test-icon.png")),
            };
            var writeService = _container.GetInstance<IWriteService>();
            var response = writeService.WriteAsync(requestHandler, icon).Result;
        }

        private static void WriteSchema(IAuthenticatedRequestHandler requestHandler)
        {
            var schema = new Schema
            {
                Name = new InternalName("cwm", "test"),
                Label = "Cwm test schema",
                RawXml = File.ReadAllText(@"C:\temp\test-schema.xml"),
            };
            var writeService = _container.GetInstance<IWriteService>();
            var response = writeService.WriteAsync(requestHandler, schema).Result;
        }

        private static void WriteSchemas(IAuthenticatedRequestHandler requestHandler)
        {
            var schemas = new[]{
                new Schema
                {
                    Name = new InternalName("cwm", "test"),
                    Label = "Cwm test schema",
                    RawXml = File.ReadAllText(@"C:\temp\test-schema.xml"),
                },
                new Schema{
                    Name=new InternalName("cwm","test2"),
                    Label="Cwm test schema 2",
                    RawXml = File.ReadAllText(@"C:\temp\test-schema2.xml"),
                },
            };
            var writeService = _container.GetInstance<IWriteService>();
            var response = writeService.WriteCollectionAsync(requestHandler, schemas).Result;
        }

        #endregion
    }
}
