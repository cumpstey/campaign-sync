using System;
using Cwm.AdobeCampaign.Testing;
using Cwm.AdobeCampaign.WebServices.Model;
using NUnit.Framework;

namespace Cwm.AdobeCampaign.WebServices.Tests.Model
{
    [TestFixture(TestOf = typeof(Schema))]
    public class SchemaTests : TestBase
    {
        [Test(Description = "Xml generated correctly for persist operation.")]
        [TestCase("Test label", "RawXml", "Case1", Description = "Generation of xml including label")]
        [TestCase(null, "RawXml", "Case2", Description = "Generation of xml without label")]
        [TestCase(null, "null", "Case3", Description = "Generation of xml without xml content")]
        public void GetXmlForPersist(string label, string inputRawXmlResource, string outputResource)
        {
            var schema = new Schema
            {
                Name = new InternalName("cwm", "test"),
                Label = label,
                RawXml = GetEmbeddedResource(inputRawXmlResource),
            };
            var expected = GetEmbeddedResource(outputResource);

            var generated = schema.GetXmlForPersist().ToString();

            Assert.AreEqual(expected, generated);
        }

        [Test(Description = "Appropriate exception thrown if name is empty.")]
        [TestCase("Test label", "RawXml", Description = "Empty name")]
        public void ThrowsOnEmptyName(string label, string inputRawXmlResource)
        {
            var schema = new Schema
            {
                Label = label,
                RawXml = GetEmbeddedResource(inputRawXmlResource),
            };

            Assert.Throws<InvalidOperationException>(() => schema.GetXmlForPersist());
        }

        [Test(Description = "Appropriate exception thrown if xml is invalid.")]
        [TestCase("Test label", "BadXml", Description = "Bad xml")]
        public void ThrowsOnBadXml(string label, string inputRawXmlResource)
        {
            var schema = new Schema
            {
                Name = new InternalName("cwm", "test"),
                Label = label,
                RawXml = GetEmbeddedResource(inputRawXmlResource),
            };

            Assert.Throws<InvalidOperationException>(() => schema.GetXmlForPersist());
        }
    }
}
