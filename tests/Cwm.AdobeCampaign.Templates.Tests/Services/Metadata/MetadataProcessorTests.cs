using System.IO;
using System.Linq;
using System.Reflection;
using Cwm.AdobeCampaign.Templates.Model;
using Cwm.AdobeCampaign.Templates.Services.Metadata;
using Cwm.AdobeCampaign.Testing;
using NUnit.Framework;

namespace Cwm.AdobeCampaign.Templates.Tests.Services.Metadata
{
    [TestFixture(TestOf = typeof(MetadataProcessor))]
    public class MetadataProcessorTests : TestBase
    {
        [Test(Description = "Parsing metadata.")]
        public void Parse()
        {
            var input = GetEmbeddedResource("Parse_Input");

            var metadataProcessor = new MetadataProcessor();
            var parsed = metadataProcessor.Parse(input);

            Assert.AreEqual("xtk:entity", parsed.Schema.ToString());
            Assert.AreEqual("cwm:test", parsed.Name.ToString());
            Assert.AreEqual("This is a test", parsed.Label);
            Assert.AreEqual(1, parsed.AdditionalProperties.Count());
            Assert.AreEqual("Property1", parsed.AdditionalProperties.First().Key);
            Assert.AreEqual("Value 1", parsed.AdditionalProperties.First().Value);
        }

        [Test(Description = "Formatting metadata.")]
        public void Format()
        {
            var metadata = new TemplateMetadata
            {
                Schema = new InternalName("xtk", "entity"),
                Name = new InternalName("cwm", "test"),
                Label = "This is a test",
            };
            metadata.AdditionalProperties.Add("Property1", "Value 1");

            var expected = GetEmbeddedResource("Format_Output");

            var metadataProcessor = new MetadataProcessor();
            var generated = metadataProcessor.Format(metadata);

            Assert.AreEqual(expected, generated);
        }
    }
}
