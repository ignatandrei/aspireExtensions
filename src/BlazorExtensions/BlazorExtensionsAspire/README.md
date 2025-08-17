# BlazorExtensionsAspire

Extension methods and helpers for integrating Blazor WebAssembly projects with .NET Aspire distributed applications.

## Features
- Blazor WebAssembly project environment integration
- Automatic appsettings.json endpoint injection for API URLs
- Utility methods for Aspire project resource management
- Simplifies connecting Blazor WebAssembly frontends to backend APIs in Aspire distributed apps

## Installation

Install via NuGet:

```bash
dotnet add package BlazorExtensionsAspire
```

## Usage

### 1. Registering a Blazor WebAssembly Project in Aspire

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
