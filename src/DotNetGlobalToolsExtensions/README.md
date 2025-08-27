# DotnetGlobalToolsExtensionAspire



[![NuGet](https://img.shields.io/nuget/v/DotnetGlobalToolsExtensionAspire.svg)](https://www.nuget.org/packages/DotnetGlobalToolsExtensionAspire/)

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

## Images

![GlobalTool](https://raw.githubusercontent.com/ignatandrei/aspireExtensions/main/docs/images/Global/FirstPage.png)

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
