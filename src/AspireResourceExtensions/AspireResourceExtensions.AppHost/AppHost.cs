using AspireResourceExtensionsAspire;
using TestExtensionsAspire;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");



var aspire = builder.AddAspireResource();
ArgumentNullException.ThrowIfNull(aspire);

var apiService = builder.AddProject<Projects.AspireResourceExtensions_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AspireResourceExtensions_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

var tests= builder.AddTestProject<Projects.AspireResourceExtensions_Tests>("MyTests",
    "test --filter Category=Integration"
    )
    .WaitFor(aspire)
    ;


aspire.Resource.AddEnvironmentVariablesTo(tests);
var app = builder.Build();
//var result = f.ParseLoginUrl(app);
var result = aspire.Resource.StartParsing(app);
await Task.WhenAll(app.RunAsync(), result);

