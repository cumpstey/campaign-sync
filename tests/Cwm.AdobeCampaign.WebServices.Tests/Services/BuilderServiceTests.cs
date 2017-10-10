using System.Xml.Linq;
using Cwm.AdobeCampaign.Testing.Dummy;
using Cwm.AdobeCampaign.WebServices.Services;
using Cwm.AdobeCampaign.WebServices.Services.Responses;
using Cwm.AdobeCampaign.WebServices.Tests.Dummy;
using NUnit.Framework;

namespace Cwm.AdobeCampaign.WebServices.Tests.Services
{
    [TestFixture(TestOf = typeof(BuilderService))]
    public class BuilderServiceTests
    {
        #region Tests of BuildNavigationHierarchy

        [Test(Description = "Handling http errors.")]
        [TestCase(ResponseStatus.ConnectionError, ResponseStatus.ConnectionError)]
        [TestCase(ResponseStatus.NotFound, ResponseStatus.NotFound)]
        [TestCase(ResponseStatus.Unauthorised, ResponseStatus.Unauthorised)]
        public void BuildNavigationHierarchy_ConnectionError(ResponseStatus requestHandlerResponse, ResponseStatus serviceResponse)
        {
            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(requestHandlerResponse));

            var response = service.BuildNavigationHierarchyAsync(requestHandler).Result;
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, serviceResponse);
        }

        [Test(Description = "Successful building of navigation hierarchy.")]
        public void BuildNavigationHierarchy_Success()
        {
            XNamespace serviceNs = string.Concat("urn:", BuilderService.ServiceNamespace);

            var responseData = new XElement(serviceNs + BuilderService.BuildNavigationHierarchyServiceName + "Response",
                new XAttribute(XNamespace.Xmlns + "urn", serviceNs));

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(ResponseStatus.Success));

            var response = service.BuildNavigationHierarchyAsync(requestHandler).Result;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(response.Status, ResponseStatus.Success);
        }

        #endregion

        #region Tests of BuildSchema

        [Test(Description = "Handling http errors.")]
        [TestCase(ResponseStatus.ConnectionError, ResponseStatus.ConnectionError)]
        [TestCase(ResponseStatus.NotFound, ResponseStatus.NotFound)]
        [TestCase(ResponseStatus.Unauthorised, ResponseStatus.Unauthorised)]
        public void BuildSchema_ConnectionError(ResponseStatus requestHandlerResponse, ResponseStatus serviceResponse)
        {
            var schemaName = new InternalName("cwm", "test");

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(requestHandlerResponse));

            var response = service.BuildSchemaAsync(requestHandler, schemaName).Result;
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, serviceResponse);
        }

        [Test(Description = "Successful building of schema.")]
        public void BuildSchema_Success()
        {
            var schemaName = new InternalName("cwm", "test");

            XNamespace serviceNs = string.Concat("urn:", BuilderService.ServiceNamespace);

            var responseData = new XElement(serviceNs + BuilderService.BuildSchemaServiceName + "Response",
                new XAttribute(XNamespace.Xmlns + "urn", serviceNs));

            var service = GetService();
            var requestHandler = new DummyRequestHandler(new Response<XElement>(ResponseStatus.Success));

            var response = service.BuildSchemaAsync(requestHandler, schemaName).Result;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(response.Status, ResponseStatus.Success);
        }

        #endregion

        #region Helpers

        private BuilderService GetService()
        {
            return new BuilderService(new DummyLoggerFactory());
        }

        #endregion
    }
}
