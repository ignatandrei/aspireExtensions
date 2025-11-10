# DocumentorDatabaseExtensionsAspire

[![NuGet](https://img.shields.io/nuget/v/DocumentorDatabaseExtensionsAspire.svg)](https://www.nuget.org/packages/DocumentorDatabaseExtensionsAspire/)

## Features

**DocumentorDatabaseExtensionsAspire** provides extension methods for .NET Aspire SQL Server hosting to automate documentation generation and integration:

- **AddDocumentationOnFolder**: Easily add documentation generation for a `SqlServerDatabaseResource` in your Aspire application. This extension:
 - Embeds and extracts documentation assets (docusaurus ) into your project folder.
 - Sets up a documentation generator project (from a GitHub repository) linked to your database resource.
 - Ensures documentation is generated and available for your database, with health checks and environment configuration.
 - Integrates repository cloning and project setup for documentation tools.

See video at

https://ignatandrei.github.io/aspireExtensions/images/DocumentorDatabaseExtensions/video-GenerateDocumentation-20251110194136.mp4

## Requirements

- .NET 9.0 or later
- .NET Aspire 9.5.0 or later
- SQL Server (via Aspire.Hosting.SqlServer)

## License

MIT License - see LICENSE.txt for details.
