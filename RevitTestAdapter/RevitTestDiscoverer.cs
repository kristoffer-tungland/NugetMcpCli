using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;

namespace RevitTestAdapter
{
    [FileExtension(".dll")]
    [DefaultExecutorUri("executor://RevitTestExecutor")]
    public class RevitTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            // Simple reflection-based discovery for sample purposes
            foreach (var source in sources)
            {
                var asm = System.Reflection.Assembly.LoadFrom(source);
                foreach (var type in asm.GetTypes())
                {
                    foreach (var method in type.GetMethods())
                    {
                        if (method.GetCustomAttributes(typeof(NUnit.Framework.TestAttribute), true).Length > 0)
                        {
                            var tc = new TestCase($"{type.FullName}.{method.Name}", new System.Uri("executor://RevitTestExecutor"), source);
                            discoverySink.SendTestCase(tc);
                        }
                    }
                }
            }
        }
    }
}
