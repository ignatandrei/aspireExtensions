using PortExtensionAspire;

var builder = DistributedApplication.CreateBuilder(args);
var ports = builder.AddPort()
    .WithDeterministicPortEnvironment("fileDisplay")
    .Construct();

var resDisplayFiles = builder.CreateFileDisplay(ports.Resource.GetDeterministicPort("fileDisplay"));
resDisplayFiles.AddFile(relativePath: "AppHost.cs", lines: ["resDisplayFiles.AddFile"]);
resDisplayFiles.AddFile(relativePath: "../tests/tests/test.spec.ts", lines: ["makeVideo"]);

var aspire = builder.AddAspireResource();

var js = builder
    .AddJavaScriptApp("tests", "../tests")
    .AddNpmCommandsFromPackage()    
    //.WithReference(ports)
    ;
aspire!.Resource.AddEnvironmentVariablesTo(js);

var app = builder.Build();
var result = aspire!.Resource.StartParsing(app, builder);
await Task.WhenAll(app.RunAsync(), result);
