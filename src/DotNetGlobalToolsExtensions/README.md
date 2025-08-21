# DotnetGlobalToolsExtensionAspire

A .NET Aspire extension for managing and integrating .NET global tools into your Aspire applications.

## Features
- Seamless integration of .NET global tools with Aspire projects
- Resource builder extensions for easy configuration
- Programmatic execution and management of global tools

## Getting Started

1. **Installation**
   - Add the `DotnetGlobalToolsExtensionAspire` package to your Aspire project.

```bash
dotnet add package SqlExtensionsAspire
```

2. **Usage**
   - Use the provided resource builder extensions to register and configure global tools in your Aspire application.

```csharp
builder.AddDotnetGlobalTools("dotnet-ef", "dotnet-depends");
```

Now a Aspire resource will be created with a command to install the specified global tools.
Also, a **All** command will be created to install all specified global tools.


## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
