# AspireFileDisplayExtension

[![NuGet](https://img.shields.io/nuget/v/AspireFileDisplayExtension.svg)](https://www.nuget.org/packages/AspireFileDisplayExtension)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](../LICENSE)

An Aspire extension that hosts a small file viewer for your distributed application. Add files to the viewer, and it serves a browser UI with syntax highlighting for supported file types.

## What it does

- Registers a single Aspire resource for displaying files
- Serves a web UI on a fixed port you choose
- Renders individual files at `/files/{name}`
- Supports Monaco-based syntax highlighting for many common extensions
- Serves embedded assets from `/vs`

## Requirements

- .NET 10.0 or later
- Aspire 13.4 or later

## Installation

```shell
dotnet add package AspireFileDisplayExtension
```

## Usage

```csharp
using AspireFileDisplayExtension;

var builder = DistributedApplication.CreateBuilder(args);

var files = builder.CreateFileDisplay(port: 55987);
files.AddFile(relativePath: "AppHost.cs", lines: ["CreateFileDisplay"]);
files.AddFile(relativePath: "..\\README.md");

builder.Build().Run();
```

Open the configured port in a browser to see the file display UI. Each added file is available under `/files/{file-name}`.

## Notes

- `AddFile` validates the file extension against the built-in Monaco language map
- If you pass `lines`, the UI marks matching lines in the file view
- The viewer is designed for local development and dashboard-style inspection

## Repository

GitHub: https://github.com/ignatandrei/aspireExtensions

