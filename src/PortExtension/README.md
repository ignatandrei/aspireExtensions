# PortExtensionsAspire

[![NuGet](https://img.shields.io/nuget/v/PortExtensionsAspire.svg)](https://www.nuget.org/packages/PortExtensionsAspire)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](../LICENSE)

A .NET Aspire extension that assigns deterministic (repeatable) ports to named resources and exposes them as `PORT_{name}` environment variables to other resources in your distributed application.

## Links

- NuGet Package: https://www.nuget.org/packages/PortExtensionsAspire
- GitHub Repository: https://github.com/ignatandrei/aspireExtensions

## Features
- Computes a deterministic port for a given name (and optional tag) so the same name always maps to the same port across runs
- Registers the computed ports on a dedicated `PortResource` so they are visible in the Aspire dashboard as environment variables
- If you need, it just add a new deterministic port with a name and value, and it will be registered in the `PortResource` for you
- Injects the registered ports (`PORT_{name}`) into any other resource via `WithPortReference`

## Requirements

- .NET 10.0 or later
- Aspire 13.0 or later

## Installation

Install via NuGet:

```shell
 dotnet add package PortExtensionsAspire
```

## Usage

```csharp
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
```

## ⚠️ Warning: ports are not guaranteed to be unique

`GetDeterministicPort` derives a port by hashing the given name (optionally combined with a tag) and reducing it into the `UInt16` range. This makes the port **repeatable** across runs for the same name, but it does **not** guarantee **uniqueness** across different names: two different names can hash to the same port (a hash collision), which would result in two resources being assigned the same port. If you hit a collision, pick a different name/tag combination for one of the resources, or manually override the port.

## Contributing

Contributions are welcome! Please open issues or submit pull requests via [GitHub](https://github.com/ignatandrei/aspireExtensions).

## License

This project is licensed under the [MIT License](../LICENSE).

## Links

- [NuGet Package](https://www.nuget.org/packages/PortExtensionsAspire)
- [GitHub Repository](https://github.com/ignatandrei/aspireExtensions)
