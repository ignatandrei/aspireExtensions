using Blazor.Extension;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.BlazorExtensions_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddWebAssemblyProject<Projects.BlazorWebAssProject>("webfrontend", apiService)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
