using TestExtensionsAspire;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.TestExtensions_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.TestExtensions_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddTestProject<Projects.TestExtensions_Tests>("SampleTests",
        "run  --filter-trait 'Category=UnitTest'",
        "run --filter-trait 'Category=Logic'");

builder.Build().Run();
