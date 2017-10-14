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
        [TestCase("Test label", "xml_input", "case1_output", Description = "Generation of xml including label")]
        [TestCase(null, "xml_input", "case2_output", Description = "Generation of xml without label")]
        [TestCase(null, null, "case3_output", Description = "Generation of xml without xml content")]
        public void GetXmlForPersist(string label, string inputFile, string outputFile)
        {
            var schema = new Schema
            {
                Name = new InternalName("cwm", "test"),
                Label = label,
                RawXml = GetEmbeddedResource(inputFile),
            };
            var expected = GetEmbeddedResource(outputFile);

            var generated = schema.GetXmlForPersist().ToString();

            Assert.AreEqual(expected, generated);
        }

        [Test(Description = "Appropriate exception thrown if name is empty.")]
        [TestCase("Test label", "xml_input", Description = "Empty name")]
        public void ThrowsOnEmptyName(string label, string inputFile)
        {
            var schema = new Schema
            {
                Label = label,
                RawXml = GetEmbeddedResource(inputFile),
            };

            Assert.Throws<InvalidOperationException>(() => schema.GetXmlForPersist());
        }

        [Test(Description = "Appropriate exception thrown if xml is invalid.")]
        [TestCase("Test label", "badXml_input", Description = "Bad xml")]
        public void ThrowsOnBadXml(string label, string inputFile)
        {
            var schema = new Schema
            {
                Name = new InternalName("cwm", "test"),
                Label = label,
                RawXml = GetEmbeddedResource(inputFile),
            };

            Assert.Throws<InvalidOperationException>(() => schema.GetXmlForPersist());
        }
    }
}
