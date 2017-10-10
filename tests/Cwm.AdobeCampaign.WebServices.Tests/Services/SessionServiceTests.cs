using System.Xml.Linq;
using Cwm.AdobeCampaign.Testing.Dummy;
using Cwm.AdobeCampaign.WebServices.Services;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using Cwm.AdobeCampaign.WebServices.Tests.Dummy;
using NUnit.Framework;

namespace Cwm.AdobeCampaign.WebServices.Tests.Services
{
    [TestFixture(TestOf = typeof(SessionService))]
    public class SessionServiceTests
    {
        #region Tests of Logon

        [Test(Description = "Handling http errors.")]
        [TestCase(ResponseStatus.ConnectionError, ResponseStatus.ConnectionError)]
        [TestCase(ResponseStatus.NotFound, ResponseStatus.NotFound)]
        [TestCase(ResponseStatus.Unauthorised, ResponseStatus.Unauthorised)]
        public void Logon_ConnectionError(ResponseStatus requestHandlerResponse, ResponseStatus serviceResponse)
        {
            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(requestHandlerResponse));

            var response = service.LogonAsync(requestHandler, string.Empty, string.Empty).Result;
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, serviceResponse);
            Assert.IsNull(response.Data);
        }

        [Test(Description = "Handling poorly formatted response data.")]
        public void Logon_ParsingError()
        {
            const string sessionToken = "__session__";
            const string securityToken = "__security__";
            XNamespace serviceNs = string.Concat("urn:", SessionService.ServiceNamespace);

            var responseData = new XElement(serviceNs + SessionService.LogonServiceName + "Response",
                new XAttribute(XNamespace.Xmlns + "urn", serviceNs),
                new XElement("pstrSessionToken", sessionToken),
                new XElement("pstrSecurityToken", securityToken));

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(ResponseStatus.Success, responseData));

            var response = service.LogonAsync(requestHandler, "", "").Result;
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, ResponseStatus.ParsingError);
            Assert.IsNull(response.Data);
        }

        [Test(Description = "Successful login.")]
        public void Logon_Success()
        {
            const string sessionToken = "__session__";
            const string securityToken = "__security__";
            XNamespace serviceNs = string.Concat("urn:", SessionService.ServiceNamespace);

            var responseData = new XElement(serviceNs + SessionService.LogonServiceName + "Response",
                new XAttribute(XNamespace.Xmlns + "urn", serviceNs),
                new XElement(serviceNs + "pstrSessionToken", sessionToken),
                new XElement(serviceNs + "pstrSecurityToken", securityToken));

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(ResponseStatus.Success, responseData));

            var response = service.LogonAsync(requestHandler, "", "").Result;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(response.Status, ResponseStatus.Success);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(response.Data.SessionToken, sessionToken);
            Assert.AreEqual(response.Data.SecurityToken, securityToken);
        }

        #endregion

        #region Helpers

        private SessionService GetService()
        {
            return new SessionService(new DummyLoggerFactory());
        }

        #endregion
    }
}
