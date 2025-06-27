# NugetMcpCli

A cross-platform .NET CLI tool and MCP server that exposes public API metadata and XML documentation from NuGet packages. Designed to give AI agents and developers structured access to types, methods, properties, parameters, and full XML documentation for any locally installed NuGet package.

## Usage

Run normally for human CLI use:

```bash
dotnet run --project NugetMcpCli -- Search Newtonsoft.Json 13.0.1 netstandard2.0 JsonConvert
```

Run as an MCP server so IDEs can discover the tool:

```bash
dotnet tool install --global NugetMcpCli
nugetmcpcli -mcp
```

Use the optional `--framework` parameter to specify the target framework when querying a package.
