using AspireResourceExtensionsAspire;
using Google.Protobuf.WellKnownTypes;
using JavaScriptExtensionsAspire;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.JavaScriptExtensions_ApiService>("apiservice")
    //.WithHttpHealthCheck("/health")
    ;

builder.AddProject<Projects.JavaScriptExtensions_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddJavaScriptApp("JavaScriptAppWithCommands","../SampleJavaScript")
    .AddNpmCommandsFromPackage()    

    
    ;
var jsTests = builder.AddJavaScriptApp("Tests", "../GenerateTests")
    .AddNpmCommandsFromPackage()
    ;

var aspire = builder.AddAspireResource();
ArgumentNullException.ThrowIfNull(aspire);
aspire.Resource.AddEnvironmentVariablesTo(jsTests);
var app = builder.Build();
var result = aspire!.Resource.StartParsing(app,builder);
await Task.WhenAll(app.RunAsync(), result);

//builder.Build().Run();


