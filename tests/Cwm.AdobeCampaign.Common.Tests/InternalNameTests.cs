using NUnit.Framework;

namespace Cwm.AdobeCampaign.Tests
{
    [TestFixture(TestOf = typeof(InternalName))]
    public class InternalNameTests
    {
        [Test]
        [TestCase("cwm:test", true, "cwm", "test")]
        [TestCase("cwm:Test", true, "cwm", "Test")]
        [TestCase("cwm:Test_123", true, "cwm", "Test_123")]
        [TestCase("cwm:test.js", true, "cwm", "test.js")]
        [TestCase("Test_123", true, "", "Test_123")]
        [TestCase("cw:Test", false, "", "")]
        [TestCase(":Test", false, "", "")]
        public void Parse(string raw, bool valid, string expectedNamespace, string expectedName)
        {
            InternalName internalName;
            try
            {
                internalName = InternalName.Parse(raw);
            }
            catch
            {
                internalName = null;
            }

            if (!valid)
            {
                Assert.IsNull(internalName);
            }
            else if (internalName == null)
            {
                Assert.Fail("Valid name, but failed to parse");
            }
            else
            {
                Assert.AreEqual(expectedNamespace, internalName.Namespace);
                Assert.AreEqual(expectedName, internalName.Name);
                Assert.AreEqual(raw, internalName.ToString());
            }
        }
    }
}
