using DotnetGlobalToolsExtensionAspire;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.DotNetGlobalToolsExtensions_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.DotNetGlobalToolsExtensions_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

//builder.AddDotnetGlobalTools(ToolName.powershell | ToolName.dotnet_project_licenses);
//builder.AddDotnetGlobalTools(toolName: DotnetGlobalToolResourceBuilderExtensions.All());
builder
    .AddDotnetGlobalTools("dotnet-ef", "dotnet-depends")
    .AddCommandTool(apiService, "dotnet-thx");
    
builder.Build().Run();
