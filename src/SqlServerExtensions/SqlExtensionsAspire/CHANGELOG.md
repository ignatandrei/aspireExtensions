# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-12-28

### Added
- Initial release of SqlExtensionsAspire
- `WithSqlCommand` extension method for executing ad-hoc SQL commands
- `ExecuteSqlServerScripts` extension method for executing SQL scripts with GO batch separator support
- `WithSqlPadViewerForDB` extension method for SQLPad integration
- Support for .NET 9.0 and .NET Aspire 9.2.0
- Comprehensive logging and error handling
- Automatic timeout configuration (120 seconds default)
- Full integration with Aspire resource management and dashboard

### Features
- Execute SQL commands from the Aspire dashboard
- Batch execution of SQL scripts with proper GO statement handling
- Web-based SQL editor integration via SQLPad container
- Automatic database connection configuration
- Resource dependency management and waiting

## [Unreleased]

### Planned
- Configurable command timeouts
- Additional database providers support
- Enhanced error reporting and diagnostics
- Custom SQLPad configuration options