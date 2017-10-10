using Cwm.AdobeCampaign.Templates.Model;
using Cwm.AdobeCampaign.Templates.Services.Metadata;

namespace Cwm.AdobeCampaign.Templates.Tests.Dummy
{
    public class DummyMetadataParser : IMetadataParser
    {
        public TemplateMetadata Parse(string input)
        {
            return new TemplateMetadata
            {
                Name = new InternalName("cwm", "dummy"),
            };
        }
    }
}
