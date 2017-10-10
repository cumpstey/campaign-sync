using Cwm.AdobeCampaign.Templates.Model;
using Cwm.AdobeCampaign.Templates.Services.Metadata;

namespace Cwm.AdobeCampaign.Templates.Tests.Dummy
{
    public class DummyMetadataFormatter : IMetadataFormatter
    {
        public string Format(TemplateMetadata metadata)
        {
            return "Dummy metadata";
        }
    }
}
