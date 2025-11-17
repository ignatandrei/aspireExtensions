using Aspire.Hosting;
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
    .WithExplicitStart()
    ;

var npmTests = builder.AddJavaScriptApp("GenerateVideo", "../GenerateTest","start")
    .WaitFor(aspire)
    .WithExplicitStart()
    ;

aspire.Resource.AddEnvironmentVariablesTo(tests,npmTests);
var app = builder.Build();
//var result = f.ParseLoginUrl(app);
var result = aspire.Resource.StartParsing(app,builder);

await Task.WhenAll(app.RunAsync(), result);

