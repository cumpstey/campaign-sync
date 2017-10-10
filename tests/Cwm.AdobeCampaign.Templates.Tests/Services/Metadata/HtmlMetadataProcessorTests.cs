using System.IO;
using System.Linq;
using System.Reflection;
using Cwm.AdobeCampaign.Templates.Model;
using Cwm.AdobeCampaign.Templates.Services.Metadata;
using Cwm.AdobeCampaign.Templates.Tests.Dummy;
using Cwm.AdobeCampaign.Testing;
using NUnit.Framework;

namespace Cwm.AdobeCampaign.Templates.Tests.Services.Metadata
{
    /// <remarks>
    /// TODO: Can't get this to work, I think because of conflicting HtmlAgilityPack references between .net framework and .net standard.
    /// </remarks>
    [TestFixture(TestOf = typeof(HtmlMetadataProcessor))]
    public class HtmlMetadataProcessorTests : TestBase
    {
        [Test(Description = "Extracting metadata.")]
        public void Extract()
        {
            var input = GetEmbeddedResource("Extract_Input");
            var output = GetEmbeddedResource("Extract_Output");

            var metadataProcessor = new HtmlMetadataProcessor(new DummyMetadataParser(), new DummyMetadataFormatter());
            var template = metadataProcessor.ExtractMetadata(input);

            Assert.IsNotNull(template.Metadata);
            Assert.AreEqual("dummy", template.Metadata.Name.Name);
            Assert.AreEqual(output, template.Code);
        }

        [Test(Description = "Extracting metadata when it doesn't exist.")]
        public void ExtractFail()
        {
            var input = GetEmbeddedResource("ExtractFail_Input");

            var metadataProcessor = new HtmlMetadataProcessor(new DummyMetadataParser(), new DummyMetadataFormatter());
            var template = metadataProcessor.ExtractMetadata(input);

            Assert.IsNull(template.Metadata);
        }

        //[Test(Description = "Formatting metadata.")]
        //public void Format()
        //{
        //    var metadata = new TemplateMetadata
        //    {
        //        Schema = new InternalName("xtk", "entity"),
        //        Name = new InternalName("cwm", "test"),
        //        Label = "This is a test",
        //    };
        //    metadata.AdditionalProperties.Add("Property1", "Value 1");

        //    var expected = GetEmbeddedResource("FormatOutput");

        //    var metadataProcessor = new MetadataProcessor();
        //    var generated = metadataProcessor.Format(metadata);

        //    Assert.AreEqual(expected, generated);
        //}
    }
}
