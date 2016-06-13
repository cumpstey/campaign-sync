using System;
using NUnit.Framework;

namespace Zone.Campaign.Tests
{
    [TestFixture]
    public class InternalNameTests
    {
        [Test]
        [TestCase("zne:test", true, "zne", "test")]
        [TestCase("zne:Test", true, "zne", "Test")]
        [TestCase("zne:Test_123", true, "zne", "Test_123")]
        [TestCase("zne:test.js", true, "zne", "test.js")]
        [TestCase("Test_123", true, "", "Test_123")]
        [TestCase("zn:Test", false, "", "")]
        [TestCase(":Test", false, "", "")]
        public void ParseTest(string raw, bool valid, string expectedNamespace, string expectedName)
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
