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
        [TestCase("Simple_Input", "Simple_Output", Description = "No transformable elements")]
        [TestCase("Include_Input", "Include_Output", Description = "Include file")]
        [TestCase("Include_t_Input", "Include_t_Output", Description = "Include file with t flag")]
        [TestCase("Include_nest_Input", "Include_nest_Output", Description = "Include file")]
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
