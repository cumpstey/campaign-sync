using System.Xml.Linq;
using Cwm.AdobeCampaign.Testing.Dummy;
using Cwm.AdobeCampaign.WebServices.Model;
using Cwm.AdobeCampaign.WebServices.Services;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using Cwm.AdobeCampaign.WebServices.Tests.Dummy;
using NUnit.Framework;

namespace Cwm.AdobeCampaign.WebServices.Tests.Services
{
    [TestFixture(TestOf = typeof(TriggeredMessageService))]
    public class TriggeredMessageServiceTests
    {
        #region Tests of PushRealTimeEvent

        [Test(Description = "Handling http errors.")]
        [TestCase(ResponseStatus.ConnectionError, ResponseStatus.ConnectionError)]
        [TestCase(ResponseStatus.NotFound, ResponseStatus.NotFound)]
        [TestCase(ResponseStatus.Unauthorised, ResponseStatus.Unauthorised)]
        public void PushRealTimeEvent_ConnectionError(ResponseStatus requestHandlerResponse, ResponseStatus serviceResponse)
        {
            var eventDetails = new Event();

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(requestHandlerResponse));

            var response = service.PushRealTimeEventAsync(requestHandler, eventDetails).Result;
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, serviceResponse);
            Assert.IsNull(response.Data);
        }

        [Test(Description = "Handling poorly formatted response data.")]
        public void PushRealTimeEvent_ParsingError()
        {
            var eventDetails = new Event();

            const int id = 123456789;
            XNamespace serviceNs = string.Concat("urn:", TriggeredMessageService.RealTimeEventServiceNamespace);

            var responseData = new XElement(serviceNs + TriggeredMessageService.PushRealTimeEventServiceName+ "Response",
                new XAttribute(XNamespace.Xmlns + "urn", serviceNs),
                new XElement("plId", id));

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(ResponseStatus.Success, responseData));

            var response = service.PushRealTimeEventAsync(requestHandler, eventDetails).Result;
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, ResponseStatus.ParsingError);
            Assert.IsNull(response.Data);
        }

        [Test(Description = "Successful event push")]
        public void PushRealTimeEvent_Success()
        {
            var eventDetails = new Event();

            const int id = 123456789;
            XNamespace serviceNs = string.Concat("urn:", TriggeredMessageService.RealTimeEventServiceNamespace);

            var responseData = new XElement(serviceNs + TriggeredMessageService.PushRealTimeEventServiceName + "Response",
                new XAttribute(XNamespace.Xmlns + "urn", serviceNs),
                new XElement(serviceNs + "plId", id));

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(ResponseStatus.Success, responseData));

            var response = service.PushRealTimeEventAsync(requestHandler, eventDetails).Result;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(response.Status, ResponseStatus.Success);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(response.Data, id);
        }

        #endregion

        #region Helpers

        private TriggeredMessageService GetService()
        {
            return new TriggeredMessageService(new DummyLoggerFactory());
        }

        #endregion
    }
}
