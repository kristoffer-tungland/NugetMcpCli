using System.IO.Pipes;
using System.IO;
using System.Text.Json;

namespace RevitAddin
{
    public static class PipeClient
    {
        private const string PipeName = "RevitTestPipe";

        public static PipeMessage? WaitForMessage()
        {
            using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut);
            client.Connect();
            using var reader = new StreamReader(client);
            var line = reader.ReadLine();
            if (line == null)
                return null;
            return JsonSerializer.Deserialize<PipeMessage>(line);
        }

        public static void SendResult(string resultPath)
        {
            using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut);
            client.Connect();
            using var writer = new StreamWriter(client) { AutoFlush = true };
            writer.WriteLine(resultPath);
        }
    }
}
