using PortExtensionAspire;

var builder = DistributedApplication.CreateBuilder(args);
var ports = builder.AddPort()
    .WithDeterministicPortEnvironment("sqliteweb", "mongodb")
    .Construct();

builder.AddSqlite("sqlite")
    .WithSqliteWeb(c =>
    {
        c.WithHttpEndpoint(targetPort: 8080, name: "http", port: ports.Resource.GetDeterministicPort("sqliteweb"));

    });

builder.AddProject<Projects.ShowPort>("ShowPort")
    .WithPortReference(ports);

builder.Build().Run();
