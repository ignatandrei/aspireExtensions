# SqlExtensionsAspire

[![NuGet](https://img.shields.io/nuget/v/SqlExtensionsAspire.svg)](https://www.nuget.org/packages/SqlExtensionsAspire/)

Extension methods for .NET Aspire SQL Server hosting that add SQL command execution, script execution, and SQLPad integration capabilities.

## Features

- SQL Command Execution: Execute ad-hoc SQL commands
- SQL Script Execution: Execute SQL scripts with GO batch separators  
- SQLPad Integration: Web-based SQL editor for database management
- Full Aspire Integration: Seamlessly integrates with .NET Aspire

## Installation

Install via NuGet:

```bash
dotnet add package SqlExtensionsAspire
```

## Usage

### Basic Setup

```csharp
using SqlExtensionsAspire;

var builder = DistributedApplication.CreateBuilder(args);
var sqlserver = builder.AddSqlServer("sqlserver");
var database = sqlserver.AddDatabase("mydatabase");
```

### SQL Command Execution

```csharp
var database = sqlserver.AddDatabase("mydatabase")
    .WithSqlCommand("clear-data", "DELETE FROM Users WHERE IsTemporary = 1")
    .WithSqlCommand("seed-data", "INSERT INTO Users (Name, Email) VALUES ('Admin', 'admin@example.com')");
```

### SQL Script Execution

```csharp
var createTableScript = @"
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL
);
GO

CREATE INDEX IX_Users_Email ON Users(Email);
GO
";

var database = sqlserver.AddDatabase("mydatabase")
    .ExecuteSqlServerScripts(createTableScript);
```

### SQLPad Integration

```csharp
var database = sqlserver.AddDatabase("mydatabase")
    .WithSqlPadViewerForDB(sqlserver);
```

## Requirements

- .NET 9.0 or later
- .NET Aspire 9.2.0 or later
- SQL Server (via Aspire.Hosting.SqlServer)

## License

MIT License - see LICENSE.txt for details.
