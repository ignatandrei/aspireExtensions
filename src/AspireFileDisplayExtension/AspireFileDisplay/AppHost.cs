using PortExtensionAspire;

var builder = DistributedApplication.CreateBuilder(args);
var ports = builder.AddPort()
    .WithDeterministicPortEnvironment("fileDisplay", "sqliteweb")
    .Construct();

var aspire = builder.AddAspireResource();

var username = builder.AddParameter("username", "sa");

var password = builder.AddParameter("password", "myPa!ssW0rd");

builder.AddSqlite("sqliteweb")
    .WithSqliteWeb(c =>
    {
        c.WithHttpEndpoint(targetPort: 8080, name: "http", port: ports.Resource.GetDeterministicPort("sqliteweb"));

    })
;

var sqlserver = builder.AddSqlServer("sqlserver", password, 1433)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDbGate()
    .WithAdminer()

;

var dbSqlServer = sqlserver.AddDatabase("vacationrelaySqlServer", "vacationrelay");

var postgres = builder.AddPostgres("postgres", username, password)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgWeb()
    .WithPgAdmin()
;

var postgresdb = postgres
    .AddDatabase("postgresdb", "vacationrelay")
    ;

var mongo = builder.AddMongoDB("mongo", userName: username, password: password)
                   .WithLifetime(ContainerLifetime.Persistent); 

var js = builder
    .AddJavaScriptApp("tests", "../tests")
    .AddNpmCommandsFromPackage()    
    //.WithReference(ports)
    ;
aspire!.Resource.AddEnvironmentVariablesTo(js);

var resDisplayFiles = builder.CreateFileDisplay(ports.Resource.GetDeterministicPort("fileDisplay"));
resDisplayFiles.AddFile(relativePath: "AppHost.cs", name: "DisplayFiles", lines: ["resDisplayFiles.AddFile"]);
resDisplayFiles.AddFile(relativePath: "../tests/tests/test.spec.ts", lines: ["makeVideo"]);
resDisplayFiles.AddFile(relativePath: "AppHost.cs", name: "postgres", lines: ["var postgres = builder.AddPostgres"]);


var app = builder.Build();
var result = aspire!.Resource.StartParsing(app, builder);
await Task.WhenAll(app.RunAsync(), result);
