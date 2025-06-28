namespace RevitAddin
{
    public class PipeMessage
    {
        public string Command { get; set; } = string.Empty;
        public string TestAssembly { get; set; } = string.Empty;
        public string[]? TestMethods { get; set; }
    }
}
