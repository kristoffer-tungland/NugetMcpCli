using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text.Json;

namespace RevitTestAdapter
{
    [ExtensionUri("executor://RevitTestExecutor")]
    public class RevitTestExecutor : ITestExecutor
    {
        private const string PipeName = "RevitTestPipe";

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var testList = new List<string>();
            foreach (var t in tests)
                testList.Add(t.FullyQualifiedName);
            RunInRevit(tests.First().Source!, testList, frameworkHandle);
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            foreach (var src in sources)
                RunInRevit(src, null, frameworkHandle);
        }

        private static void RunInRevit(string assembly, List<string>? tests, IFrameworkHandle handle)
        {
            using var server = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1);
            var psi = new ProcessStartInfo
            {
                FileName = "revit.exe",
                Arguments = $"/addin:\"{assembly}\"",
            };
            Process.Start(psi);
            server.WaitForConnection();
            using var writer = new StreamWriter(server) { AutoFlush = true };
            var msg = JsonSerializer.Serialize(new { Command = "RunTests", TestAssembly = assembly, TestMethods = tests });
            writer.WriteLine(msg);
            using var reader = new StreamReader(server);
            var resultPath = reader.ReadLine();
            if (!string.IsNullOrEmpty(resultPath))
            {
                var parser = new NUnit3TestResultParser();
                foreach (var result in parser.Parse(resultPath!, assembly))
                    handle.RecordResult(result);
            }
        }

        public void Cancel() { }
    }
}
