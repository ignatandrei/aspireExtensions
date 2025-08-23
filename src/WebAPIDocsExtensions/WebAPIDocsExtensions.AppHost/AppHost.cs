using WebAPIDocsExtensions.AppHost.sdk;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.WebAPIDocsExtensions_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.WebAPIDocsExtensions_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

apiService.AddSDKGeneration();
    
builder.Build().Run();
