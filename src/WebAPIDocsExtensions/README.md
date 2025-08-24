# WebAPIDocsExtensionAspire


[![NuGet](https://img.shields.io/nuget/v/WebAPIDocsExtensionAspire.svg)](https://www.nuget.org/packages/WebAPIDocsExtensionAspire/)

Extension methods for integrating OpenAPI/Swagger SDK generation into .NET Aspire distributed applications.

## Features
- Add OpenAPI Generator as a container resource
- Generate SDK clients for your APIs automatically
- Download generated SDKs to your project
- Logging and health check integration

## Installation

Install via NuGet:

```bash
dotnet add package WebAPIDocsExtensionAspire
```

## Usage

```csharp
using WebAPIDocsExtensionAspire;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.MyApi>("api");

builder.AddSDKGeneration_openapitools(apiService);

builder.Build().Run();
```

## Result

![Naive](https://raw.githubusercontent.com/ignatandrei/aspireExtensions/main/docs/images/WebAPIDocsExtensions/SDKNaive.jpeg)

## License

This project is licensed under the MIT License. See [LICENSE.txt](LICENSE.txt) for details.
