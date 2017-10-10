using NUnit.Framework;

namespace Cwm.AdobeCampaign.Tests
{
    [TestFixture(TestOf = typeof(ServiceName))]
    public class ServiceNameTests
    {
        [Test]
        [TestCase("cwm:ns#Name", true, "cwm:ns", "Name")]
        [TestCase("cwm:NS#Name", true, "cwm:NS", "Name")]
        [TestCase("cwm:NS_123#Name", true, "cwm:NS_123", "Name")]
        [TestCase("cwm:NS#Name_123", true, "cwm:NS", "Name_123")]
        [TestCase("cwm:NS", false, "", "")]
        [TestCase("cwm:#Name", false, "", "")]
        [TestCase("cw:NS#Test", false, "", "")]
        public void Parse(string raw, bool valid, string expectedNamespace, string expectedName)
        {
            ServiceName serviceName;
            try
            {
                serviceName = ServiceName.Parse(raw);
            }
            catch
            {
                serviceName = null;
            }

            if (!valid)
            {
                Assert.IsNull(serviceName);
            }
            else if (serviceName == null)
            {
                Assert.Fail("Valid name, but failed to parse");
            }
            else
            {
                Assert.AreEqual(expectedNamespace, serviceName.Namespace);
                Assert.AreEqual(expectedName, serviceName.Name);
                Assert.AreEqual(raw, serviceName.ToString());
            }
        }
    }
}
