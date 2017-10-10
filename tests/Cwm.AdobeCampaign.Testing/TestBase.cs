using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cwm.AdobeCampaign.Testing
{
    public abstract class TestBase
    {
        #region Fields

        //private const string RootNamespace = "Cwm.AdobeCampaign.Templates.Tests";

        #endregion

        #region Helpers

        protected string GetEmbeddedResource(string name)
        {
            var rootNamespace = GetType().Assembly.GetName().Name;
            var namespaceFragment = Regex.Replace(GetType().FullName.Replace(rootNamespace, null).TrimStart('.'), "Tests$", string.Empty);

            var assembly = Assembly.GetCallingAssembly();
            var resourceName = $"{rootNamespace}.TestResources.{namespaceFragment}.{name}.txt";

            var resources = assembly.GetManifestResourceNames();
            if (!resources.Contains(resourceName))
            {
                return null;
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion
    }
}
