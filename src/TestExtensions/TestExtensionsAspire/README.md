# TestExtensionsAspire

[![NuGet](https://img.shields.io/nuget/v/TestExtensionsAspire.svg)](https://www.nuget.org/packages/TestExtensionsAspire/)

Extension methods for .NET Aspire that add test project integration capabilities to your distributed applications.

## Features

- **Test Project Integration**: Add test projects as resources in your Aspire application
- **Command Execution**: Execute test commands with custom filters
- **Explicit Start Control**: Tests start only when explicitly triggered
- **Logging Integration**: Full integration with Aspire's logging system
- **Custom Commands**: Support for multiple test filters and custom command names

## Installation

Install via NuGet:

```bash
dotnet add package TestExtensionsAspire
```

## Usage

### Basic Setup

```csharp
using TestExtensionsAspire;

var builder = DistributedApplication.CreateBuilder(args);

// Add a test project with custom filters
builder.AddTestProject<Projects.MyApp_Tests>("MyTests",
    "run --filter-trait 'Category=UnitTest'",
    "run --filter-trait 'Category=Integration'");

builder.Build().Run();
```

### Adding Test Projects

The `AddTestProject` method allows you to add test projects to your Aspire application:

```csharp
// Add test project with multiple test commands
builder.AddTestProject<Projects.TestExtensions_Tests>("SampleTests",
    "run --filter-trait 'Category=UnitTest'",
    "run --filter-trait 'Category=Logic'",
    "run --filter-trait 'Category=Integration'");
```

### Features

- **Explicit Start**: Test projects are added with `WithExplicitStart()`, meaning they won't run automatically when the application starts
- **Custom Commands**: Each test filter becomes a separate command that can be executed independently
- **Logging**: All test output is logged through the Aspire logging system
- **Process Management**: Handles test process execution with proper timeout and error handling

### Command Options

Each test command includes:
- Custom icon (`PersonRunningFilled`)
- Display name showing the actual dotnet command
- Error handling and logging
- 5-minute timeout for test execution

## Requirements

- .NET 9.0 or later
- .NET Aspire 9.4.0 or later
- A valid test project implementing `IProjectMetadata`

## API Reference

### AddTestProject&lt;TProject&gt;

```csharp
public static IResourceBuilder<ProjectResource> AddTestProject<TProject>(
    this IDistributedApplicationBuilder builder, 
    string name, 
    params string[] arguments)
    where TProject : IProjectMetadata, new()
```

**Parameters:**
- `builder`: The distributed application builder
- `name`: Name for the test resource
- `arguments`: Array of test command arguments (e.g., dotnet test filters)

**Returns:** 
- `IResourceBuilder<ProjectResource>` that can be further configured

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.
