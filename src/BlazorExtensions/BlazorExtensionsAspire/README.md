# BlazorExtensionsAspire

[![NuGet](https://img.shields.io/nuget/v/BlazorExtensionsAspire.svg)](https://www.nuget.org/packages/BlazorExtensionsAspire/)

Extension methods and helpers for integrating Blazor WebAssembly projects with .NET Aspire distributed applications.

## Features

- Supports different environments (Development, Staging, Production, personal)
- Automatic appsettings.json endpoint injection for API URLs

## Installation

Install via NuGet:

```bash
dotnet add package BlazorExtensionsAspire
```
## How to use for different environments

### 1. Modify index.html to load environment-specific appsettings

In your Blazor WebAssembly project, modify the `wwwroot/index.html` file to load the appropriate `appsettings.{environment}.json` file based on the current environment:

```html
    <script src="_framework/blazor.webassembly.js" autostart="false"></script>
    <script>
   Blazor.start({
    environment: "does not matter"
    });
    </script>
```
### 2. Add commands for different environments in your Aspire AppHost project

in your Aspire AppHost project, add commands to set the environment when running the Blazor WebAssembly project:

```csharp
builder
    .AddWebAssemblyProject<Projects.BlazorWebAssProject>("webfrontend", apiService)
    
    .AddCommandsToModifyEnvName(new Projects.BlazorWebAssProject(),"andrei","test")
    ;
```

### 3.  Execute commands to run in different environments

Execute the following commands and the index.html will be modified to make the environment



## How to use for dynamic API endpoint injection

### 1. Registering a Blazor WebAssembly Project in Aspire and add API service in appSettings.json

In your Aspire AppHost project, use the extension method to register your Blazor WebAssembly project and link it to your API service:

```csharp
using Blazor.Extension;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.BlazorExtensions_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddWebAssemblyProject<Projects.BlazorWebAssProject>("webfrontend", apiService)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
```

This will automatically inject the API service endpoint into the Blazor WebAssembly project's `wwwroot/appsettings.json` and set the environment variable `apiservice_host`.

### 2. Consuming the API URL in Blazor WebAssembly

In your Blazor WebAssembly project, you can retrieve the API URL from configuration and register it for HTTP clients:

```csharp
var hostApi = builder.Configuration["apiservice_host"];
if (string.IsNullOrEmpty(hostApi))
{
    hostApi = builder.HostEnvironment.BaseAddress;
    if (!hostApi.EndsWith("/"))
    {
        hostApi += "/";
    }
}

builder.Services.AddKeyedScoped("api", (sp, _) => new HttpClient { BaseAddress = new Uri(hostApi) });
builder.Services.AddKeyedScoped("local_host", (sp, _) => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
```

You can then inject the correct `HttpClient` in your components using the `[Inject(Key = "api")]` attribute to call your backend API.

#### Example Razor Component

```razor

<PageTitle>Home</PageTitle>
 
<h1>API</h1>
@httpClientAPI?.BaseAddress

<h1>Local Host</h1>
@httpClientLocal?.BaseAddress
 
@code {
    [Inject(Key = "local_host")]
    public HttpClient? httpClientLocal { get; set; }

    [Inject(Key = "api")]
    public HttpClient? httpClientAPI { get; set; }
}


```

## How it works
- The `AddWebAssemblyProject` extension method links your Blazor WebAssembly frontend to a backend API resource.
- It writes the discovered API endpoint to `wwwroot/appsettings.json` and sets an environment variable (e.g., `apiservice_host`).
- Your Blazor WebAssembly app reads this value at startup and configures its `HttpClient` accordingly.
- Also, do not forget to add CORS support in your API service to allow requests from the Blazor WebAssembly frontend.
( when deploying to production, ensure proper CORS policies are set up for security or use deploy in the same site to avoid CORS ! )

## License

See [LICENSE](LICENSE) for details.
