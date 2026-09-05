using PortExtensionAspire;

var builder = DistributedApplication.CreateBuilder(args);

var ports = builder.AddPort()
    .WithDeterministicPortEnvironment("convertx")
    .Construct();
builder
    .AddContainer("Convertx", "ghcr.io/c4illin/convertx")
    .WithHttpEndpoint(ports.Resource.GetDeterministicPort("convertx"), 3000)
    .WithEnvironment("ALLOW_UNAUTHENTICATED", "true")
    ;

builder.Build().Run();
