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
    [TestFixture(TestOf = typeof(HtmlJavaScriptTemplateTransformer))]
    public class HtmlJavaScriptTemplateTransformerTests : TestBase
    {
        /// <remarks>
        /// In order to test functionality appropriately, it's necessary in some cases to combine multiple features
        /// such as empty code block with t flag. In order to keep the number of tests from spiralling, each file
        /// may contain more than one testable feature. They're split out as far as is _reasonable_ rather than _possible_.
        /// </remarks>
        [Test(Description = "Transforming template.")]
        [TestCase("Simple_Input", "Simple_Output", Description = "No transformable elements")]
        #region Comments: simple transforms
        [TestCase("Uncomment_Input", "Uncomment_Output", Description = "Uncomment markup")]
        [TestCase("RemoveComment_Input", "RemoveComment_Output", Description = "Remove comment from document")]
        [TestCase("CodeBlock_Input", "CodeBlock_Output", Description = "Transform code block")]
        [TestCase("CodeBlock_empty_Input", "CodeBlock_empty_Output", Description = "Transform empty code block")]
        [TestCase("CodeBlock_t_Input", "CodeBlock_t_Output", Description = "Transform code block with t flag")]
        [TestCase("CodeBlock_s_Input", "CodeBlock_s_Output", Description = "Transform code block with s flag")]
        [TestCase("ValueBlock_Input", "ValueBlock_Output", Description = "Transform value block")]
        [TestCase("ValueBlock_t_Input", "ValueBlock_t_Output", Description = "Transform value block with t flag")]
        [TestCase("ValueBlock_r_Input", "ValueBlock_r_Output", Description = "Transform value block with r flag")]
        [TestCase("ValueBlock_s_Input", "ValueBlock_s_Output", Description = "Transform value block with s flag")]
        #endregion
        #region Comments: complex functions
        [TestCase("IncludePre_Input", "IncludePre_Output", Description = "Include file before transform")]
        [TestCase("IncludePre_h_Input", "IncludePre_h_Output", Description = "Include file before transform with h flag")]
        [TestCase("IncludePost_Input", "IncludePost_Output", Description = "Include file after transform")]
        [TestCase("IncludePost_nest_Input", "IncludePost_nest_Output", Description = "Include file before transform with nesting")]
        [TestCase("ValueBlock_encode_Input", "ValueBlock_encode_Output", Description = "Transform value block with encode function")]
        [TestCase("Functions_Input", "Functions_Output", Description = "Render regions of document in functions")]
        #endregion
        #region Attributes
        [TestCase("Attr_Input", "Attr_Output", Description = "Add new attribute, or replace existing attribute")]
        [TestCase("Action_remove_Input", "Action_remove_Output", Description = "Remove markup")]
        [TestCase("Action_empty_Input", "Action_empty_Output", Description = "Clear content from element")]
        #endregion
        public void Transform(string inputFile, string outputFile)
        {
            var input = GetEmbeddedResource(inputFile);
            var output = GetEmbeddedResource(outputFile);

            var fileProvider = new EmbeddedResourceFileProvider(typeof(HtmlJavaScriptTemplateTransformerTests));
            var transformer = new HtmlJavaScriptTemplateTransformer(new DummyLoggerFactory(), fileProvider);
            var parameters = new TransformParameters { OriginalFileName = fileProvider.GetFullPath(inputFile) };
            var transformed = transformer.Transform(new Template { Code = input }, parameters);

            Assert.IsNotNull(transformed);
            Assert.AreEqual(1, transformed.Count());
            Assert.AreEqual(output, transformed.First().Code);
        }
    }
}
