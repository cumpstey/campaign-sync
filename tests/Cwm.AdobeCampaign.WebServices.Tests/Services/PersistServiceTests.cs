using System.Xml.Linq;
using Cwm.AdobeCampaign.Testing.Dummy;
using Cwm.AdobeCampaign.WebServices.Model;
using Cwm.AdobeCampaign.WebServices.Services;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using Cwm.AdobeCampaign.WebServices.Tests.Dummy;
using NUnit.Framework;

namespace Cwm.AdobeCampaign.WebServices.Tests.Services
{
    [TestFixture(TestOf = typeof(PersistService))]
    public class PersistServiceTests
    {
        #region Tests of Write

        [Test(Description = "Handling http errors.")]
        [TestCase(ResponseStatus.ConnectionError, ResponseStatus.ConnectionError)]
        [TestCase(ResponseStatus.NotFound, ResponseStatus.NotFound)]
        [TestCase(ResponseStatus.Unauthorised, ResponseStatus.Unauthorised)]
        public void Write_ConnectionError(ResponseStatus requestHandlerResponse, ResponseStatus serviceResponse)
        {
            var item = new Schema
            {
                Name = new InternalName("cwm", "test"),
            };

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(requestHandlerResponse));

            var response = service.WriteAsync(requestHandler, item).Result;
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, serviceResponse);
        }

        [Test(Description = "Successful write.")]
        public void Write_Success()
        {
            var item = new Schema
            {
                Name = new InternalName("cwm", "test"),
            };

            XNamespace serviceNs = string.Concat("urn:", PersistService.ServiceNamespace);

            var responseData = new XElement(serviceNs + PersistService.WriteServiceName + "Response",
                new XAttribute(XNamespace.Xmlns + "urn", serviceNs));

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(ResponseStatus.Success, responseData));

            var response = service.WriteAsync(requestHandler, item).Result;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(response.Status, ResponseStatus.Success);
        }

        #endregion

        #region Tests of WriteCollection

        [Test(Description = "Handling http errors.")]
        [TestCase(ResponseStatus.ConnectionError, ResponseStatus.ConnectionError)]
        [TestCase(ResponseStatus.NotFound, ResponseStatus.NotFound)]
        [TestCase(ResponseStatus.Unauthorised, ResponseStatus.Unauthorised)]
        public void WriteCollection_ConnectionError(ResponseStatus requestHandlerResponse, ResponseStatus serviceResponse)
        {
            var items = new[] {
                new Schema
                {
                    Name = new InternalName("cwm", "test"),
                }
            };

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(requestHandlerResponse));

            var response = service.WriteCollectionAsync(requestHandler, items).Result;
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, serviceResponse);
        }

        [Test(Description = "Successful write.")]
        public void WriteCollection_Success()
        {
            var items = new[] {
                new Schema
                {
                    Name = new InternalName("cwm", "test"),
                }
            };

            XNamespace serviceNs = string.Concat("urn:", PersistService.ServiceNamespace);

            var responseData = new XElement(serviceNs + PersistService.WriteServiceName + "Response",
                new XAttribute(XNamespace.Xmlns + "urn", serviceNs));

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(ResponseStatus.Success, responseData));

            var response = service.WriteCollectionAsync(requestHandler, items).Result;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(response.Status, ResponseStatus.Success);
        }

        #endregion

        #region Helpers

        private PersistService GetService()
        {
            return new PersistService(new DummyLoggerFactory());
        }

        #endregion
    }
}
