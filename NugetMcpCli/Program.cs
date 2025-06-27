using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = Host.CreateApplicationBuilder(args);
var mcp = builder.Services.AddMcpServer()
    .WithStdioServerTransport();

// If `-mcp` present, enable tool registration
if (args.Contains("-mcp"))
    mcp.WithTools<NugetDocTools>();

builder.Logging.AddConsole(opts =>
{
    opts.LogToStandardErrorThreshold = LogLevel.Trace;
});

await builder.Build().RunAsync();

[McpServerToolType]
public class NugetDocTools
{
    [McpServerTool, Description("Search members in a NuGet package")]
    public static object Search(
        string packageId,
        string version,
        string framework = "net8.0",
        string query = "")
        => MetadataService.Search(packageId, version, framework, query);

    [McpServerTool, Description("Get full signature and XML doc of a member")]
    public static object GetSignature(
        string packageId,
        string version,
        string framework,
        string fullMemberName)
        => MetadataService.GetMemberDetails(packageId, version, framework, fullMemberName);
}
