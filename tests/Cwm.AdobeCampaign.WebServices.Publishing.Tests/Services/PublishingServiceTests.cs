using System.Linq;
using System.Xml.Linq;
using Cwm.AdobeCampaign.Testing.Dummy;
using Cwm.AdobeCampaign.WebServices.Services;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using Cwm.AdobeCampaign.WebServices.Tests.Dummy;
using NUnit.Framework;

namespace Cwm.AdobeCampaign.WebServices.Tests.Services
{
    [TestFixture(TestOf = typeof(PublishingService))]
    public class PublishingServiceTests
    {
        #region Tests of PublishTriggeredMessageInstances

        [Test(Description = "Handling http errors.")]
        [TestCase(ResponseStatus.ConnectionError, ResponseStatus.ConnectionError)]
        [TestCase(ResponseStatus.NotFound, ResponseStatus.NotFound)]
        [TestCase(ResponseStatus.Unauthorised, ResponseStatus.Unauthorised)]
        public void PublishTriggeredMessageInstances_ConnectionError(ResponseStatus requestHandlerResponse, ResponseStatus serviceResponse)
        {
            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(requestHandlerResponse));

            var response = service.PublishTriggeredMessageInstancesAsync(requestHandler).Result;
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, serviceResponse);
            Assert.IsNull(response.Data);
        }

        [Test(Description = "Handling poorly formatted response data.")]
        public void PublishTriggeredMessageInstances_ParsingError()
        {
            XNamespace serviceNs = string.Concat("urn:", PublishingService.ServiceNamespace);

            var responseData = new XElement(serviceNs + PublishingService.PublishTriggeredMessageInstancesServiceName + "Response",
                new XAttribute(XNamespace.Xmlns + "urn", serviceNs),
                new XElement("deliveries",
                    new XElement("delivery", new XAttribute("id", 1), new XAttribute("success", true),
                    new XElement("delivery", new XAttribute("id", 2), new XAttribute("success", false)))));

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(ResponseStatus.Success, responseData));

            var response = service.PublishTriggeredMessageInstancesAsync(requestHandler).Result;
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, ResponseStatus.ParsingError);
            Assert.IsNull(response.Data);
        }

        [Test(Description = "Successful login.")]
        public void PublishTriggeredMessageInstances_Success()
        {
            XNamespace serviceNs = string.Concat("urn:", PublishingService.ServiceNamespace);

            var responseData = new XElement(serviceNs + PublishingService.PublishTriggeredMessageInstancesServiceName + "Response",
                new XAttribute(XNamespace.Xmlns + "urn", serviceNs),
                new XElement(serviceNs + "deliveries",
                    new XElement(serviceNs + "delivery", new XAttribute("id", 1), new XAttribute("success", true)),
                    new XElement(serviceNs + "delivery", new XAttribute("id", 2), new XAttribute("success", false))));

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(ResponseStatus.Success, responseData));

            var response = service.PublishTriggeredMessageInstancesAsync(requestHandler).Result;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(response.Status, ResponseStatus.Success);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(response.Data.Count(), 2);
            Assert.AreEqual(response.Data.ElementAt(0).Key, 1);
            Assert.AreEqual(response.Data.ElementAt(0).Value, true);
            Assert.AreEqual(response.Data.ElementAt(1).Key, 2);
            Assert.AreEqual(response.Data.ElementAt(1).Value, false);
        }

        #endregion

        #region Helpers

        private PublishingService GetService()
        {
            return new PublishingService(new DummyLoggerFactory());
        }

        #endregion
    }
}
