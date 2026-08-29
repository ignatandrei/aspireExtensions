using AspireResourceExtensionsAspire;
using JavaScriptExtensionsAspire;
using PortExtensionAspire;

var builder = DistributedApplication.CreateBuilder(args);
var ports = builder.AddPort()
    .WithDeterministicPortEnvironment("sqliteweb", "mongodb")
    .WithDeterministicPortEnvironment("andrei", 12345)
    .Construct();


var aspire = builder.AddAspireResource();


builder.AddSqlite("sqlite")
    .WithSqliteWeb(c =>
    {
        c.WithHttpEndpoint(targetPort: 8080, name: "http", port: ports.Resource.GetDeterministicPort("sqliteweb"));

    });

var project = builder.AddProject<Projects.ShowPort>("ShowPort")
    .WithPortReference(ports);

var js = builder
    .AddJavaScriptApp("GenerateTests", "../GenerateTests")
    .AddNpmCommandsFromPackage();

aspire!.Resource.AddEnvironmentVariablesTo(js);

//builder.Build().Run
var app = builder.Build();
var result = aspire!.Resource.StartParsing(app, builder);
await Task.WhenAll(app.RunAsync(), result);
