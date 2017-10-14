using System.IO;
using System.Linq;
using System.Reflection;
using Cwm.AdobeCampaign.Templates.Exceptions;
using Cwm.AdobeCampaign.Templates.Model;
using Cwm.AdobeCampaign.Templates.Services.Metadata;
using Cwm.AdobeCampaign.Templates.Services.Transforms;
using Cwm.AdobeCampaign.Templates.Tests.Dummy;
using Cwm.AdobeCampaign.Testing;
using Cwm.AdobeCampaign.Testing.Dummy;
using NUnit.Framework;

namespace Cwm.AdobeCampaign.Templates.Tests.Services.Transforms
{
    [TestFixture(TestOf = typeof(JsspJavaScriptTemplateTransformer))]
    public class JsspJavaScriptTemplateTransformerTests : TestBase
    {
        [Test(Description = "Transforming template.")]
        [TestCase("simple_input", "simple_output", Description = "No transformable elements")]
        [TestCase("include_input", "include_output", Description = "Include file")]
        [TestCase("include_t_input", "include_t_output", Description = "Include file with t flag")]
        [TestCase("include_nest_input", "include_nest_output", Description = "Include file")]
        public void Transform(string inputFile, string outputFile)
        {
            var input = GetEmbeddedResource(inputFile);
            var output = GetEmbeddedResource(outputFile);

            var fileProvider = new EmbeddedResourceFileProvider(typeof(JsspJavaScriptTemplateTransformerTests));
            var transformer = new JsspJavaScriptTemplateTransformer(new DummyLoggerFactory(), fileProvider);
            var parameters = new TransformParameters { OriginalFileName = fileProvider.GetFullPath(inputFile) };
            var transformed = transformer.Transform(new Template { Code = input }, parameters);

            Assert.IsNotNull(transformed);
            Assert.AreEqual(1, transformed.Count());
            Assert.AreEqual(output, transformed.First().Code);
        }
    }
}
