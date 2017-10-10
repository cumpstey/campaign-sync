using System.IO;
using System.Linq;
using System.Reflection;
using Cwm.AdobeCampaign.Templates.Exceptions;
using Cwm.AdobeCampaign.Templates.Model;
using Cwm.AdobeCampaign.Templates.Services.Metadata;
using Cwm.AdobeCampaign.Templates.Tests.Dummy;
using Cwm.AdobeCampaign.Testing;
using NUnit.Framework;

namespace Cwm.AdobeCampaign.Templates.Tests.Services.Metadata
{
    [TestFixture(TestOf = typeof(JavaScriptMetadataProcessor))]
    public class JavaScriptMetadataProcessorTests : TestBase
    {
        [Test(Description = "Extracting metadata.")]
        public void Extract()
        {
            var input = GetEmbeddedResource("Extract_Input");
            var output = GetEmbeddedResource("Extract_Output");

            var metadataProcessor = new JavaScriptMetadataProcessor(new DummyMetadataParser(), new DummyMetadataFormatter());
            var template = metadataProcessor.ExtractMetadata(input);

            Assert.IsNotNull(template.Metadata);
            Assert.AreEqual("dummy", template.Metadata.Name.Name);
            Assert.AreEqual(output, template.Code);
        }

        [Test(Description = "Extracting metadata when it doesn't exist.")]
        public void ExtractNoMetadata()
        {
            var input = GetEmbeddedResource("ExtractNoMetadata_Input");

            var metadataProcessor = new JavaScriptMetadataProcessor(new DummyMetadataParser(), new DummyMetadataFormatter());
            var template = metadataProcessor.ExtractMetadata(input);

            Assert.IsNotNull(template.Metadata);
            Assert.IsNull(template.Metadata.Schema);
            Assert.IsNull(template.Metadata.Name);
        }

        [Test(Description = "Extracting multiple metadata blocks.")]
        public void ExtractMultipleMetadata()
        {
            var input = GetEmbeddedResource("ExtractMultipleMetadata_Input");

            var metadataProcessor = new JavaScriptMetadataProcessor(new DummyMetadataParser(), new DummyMetadataFormatter());

            Assert.Throws<MultipleMetadataException>(() =>
            {
                var template = metadataProcessor.ExtractMetadata(input);
            });
        }

        [Test(Description = "Inserting metadata.")]
        public void Format()
        {
            var template = new Template
            {
                Code = GetEmbeddedResource("Insert_Input"),
                Metadata = new TemplateMetadata(),
            };

            var expected = GetEmbeddedResource("Insert_Output");

            var metadataProcessor = new JavaScriptMetadataProcessor(new DummyMetadataParser(), new DummyMetadataFormatter());
            var generated = metadataProcessor.InsertMetadata(template);

            Assert.AreEqual(expected, generated);
        }
    }
}
