using System;
using NUnit.Framework;

namespace Zone.Campaign.Tests
{
    [TestFixture]
    public class ServiceNameTests
    {
        [Test]
        [TestCase("zne:ns#Name", true, "zne:ns", "Name")]
        [TestCase("zne:NS#Name", true, "zne:NS", "Name")]
        [TestCase("zne:NS_123#Name", true, "zne:NS_123", "Name")]
        [TestCase("zne:NS#Name_123", true, "zne:NS", "Name_123")]
        [TestCase("zne:NS", false, "", "")]
        [TestCase("zne:#Name", false, "", "")]
        [TestCase("zn:NS#Test", false, "", "")]
        public void ParseTest(string raw, bool valid, string expectedNamespace, string expectedName)
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
