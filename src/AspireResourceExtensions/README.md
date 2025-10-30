# AspireResourceExtensions.Aspire

This package provides extensions for integrating Aspire resources into your distributed application, making it easier to test and manage Aspire dashboards.

## Installation

Install via NuGet:

```
dotnet add package AspireResourceExtensions.Aspire
```

## Usage

1. Add the Aspire resource to your distributed application builder:

```csharp
using AspireResourceExtensionsAspire;

var aspire = builder.AddAspireResource();
```

2. Use the resource to add environment variables to other resources:

```csharp
aspire.Resource.AddEnvironmentVariablesTo(otherResource);
```

3. Start parsing Aspire dashboard URLs:

```csharp
var result = aspire.Resource.StartParsing(app);
await Task.WhenAll(app.RunAsync(), result);
```

## Features
- Adds Aspire dashboard as a resource for testing
- Exposes login and base URLs as environment variables
- Designed for .NET Aspire distributed applications

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
