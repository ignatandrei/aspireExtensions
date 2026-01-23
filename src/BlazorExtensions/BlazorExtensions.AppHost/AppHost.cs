using Blazor.Extension;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.BlazorExtensions_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder
    .AddWebAssemblyProject<Projects.BlazorWebAssProject>("webfrontend", apiService)
    
    .AddCommandsToModifyEnvName(new Projects.BlazorWebAssProject(),"andrei","test")
    ;

builder.Build().Run();
