using AspireResourceExtensionsAspire;
using DocumentorDatabaseExtensionsAspire;
using SqlExtensionsAspire;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.DocumentorDatabaseExtensions_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.DocumentorDatabaseExtensions_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);


var sqlserver = builder.AddSqlServer("sql")
                 .WithLifetime(ContainerLifetime.Persistent);
var filePubs=await File.ReadAllTextAsync("instpubs.sql");
var db = sqlserver.AddDatabase("pubs")
    .ExecuteSqlServerScriptsAtStartup(filePubs)
      .WithSqlPadViewerForDB(sqlserver);
//accepts also relative paths
var res = db.AddDocumentationOnFolder(@"D:\documentation");


var aspire = builder.AddAspireResource();
var tests = builder
    .AddNpmApp("tests", "../GenerateTest")
    .WithExplicitStart()
    .WaitFor(aspire!)
    ;

aspire!.Resource.AddEnvironmentVariablesTo(tests);

var app = builder.Build();
//builder.Build().Run();
var result = aspire.Resource.StartParsing(app);
await Task.WhenAll(app.RunAsync(), result);
