# JavaScriptExtensionsAspire

[![NuGet](https://img.shields.io/nuget/v/JavaScriptExtensionsAspire.svg)](https://www.nuget.org/packages/JavaScriptExtensionsAspire)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](../LICENSE)

A .NET Aspire extension for integrating JavaScript resource management and package.json commands into your distributed applications.

## Features
- Integrates package.json commands for streamlined JavaScript workflows

## Requirements

- .NET 10.0 or later
- Aspire 13.0 or later
- 
## Installation

Install via NuGet:

```shell
 dotnet add package JavaScriptExtensionsAspire
```

## Usage

```csharp

builder
    .AddJavaScriptApp("JavaScriptAppWithCommands","../SampleJavaScript")
    .AddNpmCommandsFromPackage()    

```


See the how it looks in the 

https://ignatandrei.github.io/aspireExtensions/images/JavaScriptExtensions/packageJson.mp4

![ShowUrl](https://ignatandrei.github.io/aspireExtensions/images/JavaScriptExtensions/packageJson.mp4)

## Contributing

Contributions are welcome! Please open issues or submit pull requests via [GitHub](https://github.com/ignatandrei/aspireExtensions).

## License

This project is licensed under the [MIT License](../LICENSE).

## Links

- [NuGet Package](https://www.nuget.org/packages/JavaScriptExtensionsAspire)
- [GitHub Repository](https://github.com/ignatandrei/aspireExtensions)
