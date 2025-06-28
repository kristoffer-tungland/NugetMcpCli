using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.Xml.Linq;

namespace RevitTestAdapter
{
    internal class NUnit3TestResultParser
    {
        public IEnumerable<TestResult> Parse(string resultXmlPath, string assembly)
        {
            var doc = XDocument.Load(resultXmlPath);
            var testCases = doc.Descendants("test-case");
            foreach (var test in testCases)
            {
                var name = test.Attribute("fullname")!.Value;
                var outcome = test.Attribute("result")!.Value;
                var duration = double.Parse(test.Attribute("duration")!.Value);
                var tc = new TestCase(name, new System.Uri("executor://RevitTestExecutor"), assembly);
                var tr = new TestResult(tc)
                {
                    Duration = System.TimeSpan.FromSeconds(duration),
                    Outcome = outcome == "Passed" ? TestOutcome.Passed : outcome == "Failed" ? TestOutcome.Failed : TestOutcome.Skipped
                };
                if (outcome == "Failed")
                {
                    tr.ErrorMessage = test.Element("failure")?.Element("message")?.Value;
                    tr.ErrorStackTrace = test.Element("failure")?.Element("stack-trace")?.Value;
                }
                yield return tr;
            }
        }
    }
}
