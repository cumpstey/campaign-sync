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
        [TestCase("simple_input", "simple_output", Description = "No transformable elements")]
        #region Comments: simple transforms
        [TestCase("uncomment_input", "uncomment_output", Description = "Uncomment markup")]
        [TestCase("removeComment_input", "removeComment_output", Description = "Remove comment from document")]
        [TestCase("codeBlock_input", "codeBlock_output", Description = "Transform code block")]
        [TestCase("codeBlock_empty_input", "codeBlock_empty_output", Description = "Transform empty code block")]
        [TestCase("codeBlock_t_input", "codeBlock_t_output", Description = "Transform code block with t flag")]
        [TestCase("codeBlock_s_input", "codeBlock_s_output", Description = "Transform code block with s flag")]
        [TestCase("valueBlock_input", "valueBlock_output", Description = "Transform value block")]
        [TestCase("valueBlock_t_input", "valueBlock_t_output", Description = "Transform value block with t flag")]
        [TestCase("valueBlock_r_input", "valueBlock_r_output", Description = "Transform value block with r flag")]
        [TestCase("valueBlock_s_input", "valueBlock_s_output", Description = "Transform value block with s flag")]
        #endregion
        #region Comments: complex functions
        [TestCase("includePre_input", "includePre_output", Description = "Include file before transform")]
        [TestCase("includePre_h_input", "includePre_h_output", Description = "Include file before transform with h flag")]
        [TestCase("includePost_input", "includePost_output", Description = "Include file after transform")]
        [TestCase("includePost_nest_input", "includePost_nest_output", Description = "Include file after transform with nested includes")]
        [TestCase("valueBlock_encode_input", "valueBlock_encode_output", Description = "Transform value block with encode function")]
        [TestCase("functions_input", "functions_output", Description = "Render regions of document in functions")]
        [TestCase("functions_nest_input", "functions_nest_output", Description = "Render regions of document in functions, with nested function declarations")]
        #endregion
        #region Attributes
        [TestCase("attr_input", "attr_output", Description = "Add new attribute, or replace existing attribute")]
        [TestCase("action_remove_input", "action_remove_output", Description = "Remove markup")]
        [TestCase("action_empty_input", "action_empty_output", Description = "Clear content from element")]
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
