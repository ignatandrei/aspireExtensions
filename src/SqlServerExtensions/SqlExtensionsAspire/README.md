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
var db = sqlserver.AddDatabase("DepEmp")
    .WithSqlCommand("deleteEmployee","delete from Employee", ExecCommandEnum.NonQuery)
    .WithSqlCommand("selectEmployeeCount", "select count(*) from Employee", ExecCommandEnum.Scalar)
```


See above link with demo for executing commands - there are 2 employees, then, after delete command, there is 0 employee.

https://ignatandrei.github.io/aspireExtensions/images/SqlServerExtensions/video-Execute_SqlCommand-20251102225723.mp4


## Recreate Database at initial state with scripts
```csharp
var db = sqlserver.AddDatabase("DepEmp")
    .WithSqlCommand("deleteEmployee","delete from Employee", ExecCommandEnum.NonQuery) //just for demo
    .WithSqlCommand("selectEmployeeCount", "select count(*) from Employee", ExecCommandEnum.Scalar) //just for demo
    .ExecuteSqlServerScriptsAtStartup(DBFiles.FilesToCreate.ToArray())

```


See above link with demo for recreate database with  scripts whenever you want.

https://ignatandrei.github.io/aspireExtensions/images/SqlServerExtensions/video-Recreate_Database_With_Scripts-20251102225854.mp4


### SQLPad Integration

```csharp
var database = sqlserver.AddDatabase("mydatabase")
    .WithSqlPadViewerForDB(sqlserver);
```

## Requirements

- .NET 9.0 or later
- .NET Aspire 9.5.0 or later
- SQL Server (via Aspire.Hosting.SqlServer)

## License

MIT License - see LICENSE.txt for details.
