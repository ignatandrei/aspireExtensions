using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlExtensionsAspire;

/// <summary>
/// Extension methods for SQL Server resources in .NET Aspire applications.
/// Provides functionality for executing SQL commands, adding SQL Pad viewer, and running SQL scripts.
/// </summary>
public static partial class SqlServerExtensions
{
    /// <summary>
    /// Adds a custom SQL command to a SQL Server database resource that can be executed through the Aspire dashboard.
    /// </summary>
    /// <param name="db">The SQL Server database resource builder to extend</param>
    /// <param name="name">The name of the command as it will appear in the dashboard</param>
    /// <param name="sql">The SQL script to execute when the command is invoked</param>
    /// <param name="commandOptions">Optional command configuration options</param>
    /// <returns>The same resource builder for method chaining</returns>
    public static IResourceBuilder<SqlServerDatabaseResource> WithSqlCommand(this IResourceBuilder<SqlServerDatabaseResource> db, string name, string sql, ExecCommandEnum exec, CommandOptions? commandOptions = null)
    {

        db.WithCommand(name, name, async ecc =>
        {
            // Get the resource logger service to log command execution
            var log = ecc.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
            if (log == null) return new ExecuteCommandResult()
            {
                Success = false,
                ErrorMessage = $"no logging available"
            };

            // Get a logger instance for this specific database resource
            var lg = log.GetLogger(db.Resource);
            if (lg == null) return new ExecuteCommandResult()
            {
                Success = false,
                ErrorMessage = $"on {db.Resource.Name} no logger"
            };
            var data = await ExecuteSqlScripts( db.Resource, lg, CancellationToken.None, exec, new[] { sql });

            if(data) 
            {
                lg.LogDebug($"Executed command {name} on database {db.Resource.Name}");
                return CommandResults.Success();
            }
            else
            {
                lg.LogError($"Failed to execute command {name} on database {db.Resource.Name}");
                return CommandResults.Failure($"Failed to execute command {name}");
            }

        }, commandOptions);
        return db;
    }

    /// <summary>
    /// Adds a SQLPad web-based SQL editor and query tool for the specified database.
    /// SQLPad provides a web interface for running SQL queries and visualizing results.
    /// </summary>
    /// <param name="db">The SQL Server database resource builder to extend</param>
    /// <param name="sqlserver">The SQL Server instance that hosts the database</param>
    /// <returns>The same database resource builder for method chaining</returns>
    public static IResourceBuilder<SqlServerDatabaseResource> WithSqlPadViewerForDB(this IResourceBuilder<SqlServerDatabaseResource> db, IResourceBuilder<SqlServerServerResource> sqlserver)
    {
        var builder = db.ApplicationBuilder;
        var sqlServerName = sqlserver.Resource.Name;
        // Use the ParameterResource directly for environment variables instead of .Value
        var passwordParameter = sqlserver.Resource.PasswordParameter;
        // Create SQLPad container with pre-configured connections
        var sqlpad = builder
            .AddContainer("sqlpad", "sqlpad/sqlpad:latest")
            .WithEndpoint(5600, 3000, "http")
            // Disable authentication for development ease
            .WithEnvironment("SQLPAD_AUTH_DISABLED", "true")
            .WithEnvironment("SQLPAD_AUTH_DISABLED_DEFAULT_ROLE", "Admin")
            .WithEnvironment("SQLPAD_ADMIN", "admin@sqlpad.com")
            .WithEnvironment("SQLPAD_ADMIN_PASSWORD", "admin")
            // Configure connection to the target database
            .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo__name", sqlServerName)
            .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo__driver", "sqlserver")
            .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo__host", sqlServerName)
            .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo__database", db.Resource.Name)
            .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo__username", "sa")
            .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo__password", passwordParameter)
            // Configure connection to the master database for server-level operations
            .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo1__name", "SqlMaster")
            .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo1__driver", "sqlserver")
            .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo1__host", sqlServerName)
            .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo1__database", "master")
            .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo1__username", "sa")
            .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo1__password", passwordParameter)
            // Ensure SQLPad starts after the database and server are ready
            .WithParentRelationship(db)
            .WaitFor(db)
            .WaitFor(sqlserver)
            ;
        return db;
    }


    /// <summary>
    /// Regex pattern to match SQL Server GO statements that are used to separate batches.
    /// Supports optional repeat count (e.g., "GO 5" to repeat the batch 5 times).
    /// Reference: https://learn.microsoft.com/sql/t-sql/language-elements/sql-server-utilities-statements-go
    /// </summary>
    [GeneratedRegex(@"^\s*GO(?<repeat>\s+\d{1,6})?(\s*\-{2,}.*)?\s*$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    internal static partial Regex GoStatements();

    /// <summary>
    /// Executes multiple SQL scripts against the database when the resource becomes ready.
    /// Scripts are parsed to handle GO statements properly, splitting them into separate batches.
    /// This method subscribes to the ResourceReadyEvent to ensure scripts run after the database is available.
    /// </summary>
    /// <param name="db">The SQL Server database resource builder to extend</param>
    /// <param name="sqlScripts">Collection of SQL script strings to execute</param>
    /// <returns>The same database resource builder for method chaining</returns>
    public static IResourceBuilder<SqlServerDatabaseResource> ExecuteSqlServerScriptsAtStartup(this IResourceBuilder<SqlServerDatabaseResource> db,  params string[] sqlScripts)
    {
        var builder = db.ApplicationBuilder;
        DropCreateDBCommand(db);
        ExecScripts(db, sqlScripts);
        RecreateWithScripts(db, sqlScripts);
        // Subscribe to the resource ready event to execute scripts when database is available
        db.OnResourceReady(async (dbRes, ev, ct) =>
        {
            // Get the database connection string
            var cn = await dbRes.ConnectionStringExpression.GetValueAsync(ct);
            if (cn == null) return;

            // Set up logging for script execution
            var log = ev.Services.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
            if (log == null)
            {
                Console.WriteLine("No ResourceLoggerService");
                return;
            }
            var lg = log.GetLogger(db.Resource);
            if (lg == null)
            {
                Console.WriteLine($"No logger for {db.Resource.Name}");
                return;
            }

            // Open connection to the database
            await ExecuteSqlScripts( dbRes, lg, ct, ExecCommandEnum.NonQuery, sqlScripts);
        });
        return db;
    }

    private static async Task<bool> ExecuteSqlScripts(SqlServerDatabaseResource dbRes,  ILogger? lg, CancellationToken ct, ExecCommandEnum execCommandEnum, params string[] scripts)
    {
        var sqlScripts = scripts?.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        if (sqlScripts == null || sqlScripts.Length == 0) return false;
        var cn = await dbRes.ConnectionStringExpression.GetValueAsync(ct);
        if (cn == null) return false;
        using var sqlConnection = new SqlConnection();
        sqlConnection.ConnectionString = cn;
        await sqlConnection.OpenAsync(ct);
        if(ct.IsCancellationRequested) return false;
        int nr = 0;
        // Process each SQL script
        foreach (var item in sqlScripts)
        {
            if (ct.IsCancellationRequested) return false;
            lg?.LogDebug($"Executing script {++nr}");
            using var reader = new StringReader(item);
            var batchBuilder = new StringBuilder();

            // Parse the script line by line to handle GO statements
            while (reader.ReadLine() is { } line)
            {
                var matchGo = GoStatements().Match(line);

                if (matchGo.Success)
                {
                    // Execute the current batch when GO statement is found
                    var count = matchGo.Groups["repeat"].Success ? int.Parse(matchGo.Groups["repeat"].Value, CultureInfo.InvariantCulture) : 1;
                    var batch = batchBuilder.ToString();
                    if (string.IsNullOrWhiteSpace(batch))
                    {
                        batchBuilder.Clear();
                        continue;
                    }

                    // Execute the batch the specified number of times (default: 1)
                    for (var i = 0; i < count; i++)
                    {
                        using var command = sqlConnection.CreateCommand();
                        command.CommandText = batch;
                        // Set command timeout to 2 minutes (TODO: make this configurable)
                        command.CommandTimeout = 120;
                        try
                        {
                            switch(execCommandEnum)
                            {
                                case ExecCommandEnum.None:
                                    throw new InvalidOperationException("ExecCommandEnum.None is not valid for execution");
                                case ExecCommandEnum.NonQuery:
                                    var nrRows = await command.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
                                    lg?.LogInformation($"Executed batch (GO {i + 1} from {count}), affected rows: \u001b[38;5;1m{nrRows}\u001b[0m");
                                    continue;
                                case ExecCommandEnum.Scalar:
                                    var scalarResult = await command.ExecuteScalarAsync(ct).ConfigureAwait(false);
                                    lg?.LogInformation($"Executed batch (GO {i + 1} from {count}), scalar result: \u001b[38;5;1m{scalarResult}\u001b[0m");
                                    continue;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(execCommandEnum), execCommandEnum, null);
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            // Log errors but continue with remaining scripts
                            lg?.LogError(ex, $"!!!!Error in executing {batch} :\r\n !!!! {ex.Message}");
                        }
                    }

                    // Clear the batch builder for the next batch
                    batchBuilder.Clear();
                }
                else
                {
                    // Add non-empty lines to the current batch
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        batchBuilder.AppendLine(line);
                    }
                }
            }

            // Process any remaining batch lines after the last GO statement
            if (batchBuilder.Length > 0)
            {
                if(sqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await sqlConnection.OpenAsync(ct);
                }
                using var command = sqlConnection.CreateCommand();
                var batch = batchBuilder.ToString();
                if (string.IsNullOrWhiteSpace(batch))
                {
                    batchBuilder.Clear();
                    continue;
                }
                command.CommandText = batch;
                // Set command timeout (TODO: make this configurable)
                try
                {
                    switch (execCommandEnum)
                    {
                        case ExecCommandEnum.None:
                            throw new InvalidOperationException("ExecCommandEnum.None is not valid for execution");
                        case ExecCommandEnum.NonQuery:
                            var nrRows = await command.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
                            lg?.LogInformation($"Executed batch (final), affected rows: \u001b[38;5;1m{nrRows}\u001b[0m");
                            continue;
                        case ExecCommandEnum.Scalar:
                            var scalarResult = await command.ExecuteScalarAsync(ct).ConfigureAwait(false);
                            lg?.LogInformation($"Executed batch (final), scalar result: \u001b[38;5;1m{scalarResult}\u001b[0m");
                            continue;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(execCommandEnum), execCommandEnum, null);
                    }
                }
                catch (Exception ex)
                {
                    // Log errors for the final batch
                    lg?.LogError(ex, $"!!!!Error in executing {batch} :\r\n !!!! {ex.Message}");
                }
            }
        }
        lg?.LogDebug($"Executed {nr} scripts on database {dbRes.Name}");
        return true;
    }
    private static IResourceBuilder<SqlServerDatabaseResource> ExecScripts(this IResourceBuilder<SqlServerDatabaseResource> db, params string[] sqlScripts)
    {
        var arr = sqlScripts?.ToArray();
        if(arr == null || arr.Length == 0) return db;
        string sqlAll = string.Join("\r\nGO\r\n", arr);
        db.WithSqlCommand("Startup_ExecScripts",sqlAll,ExecCommandEnum.NonQuery, new CommandOptions()
        {
            Description = $"Execute {arr.Length} startup scripts on database {db.Resource.DatabaseName}",
            IconName = "TextBulletListSquareWarning",
        });
        return db;
    }
    static IResourceBuilder<SqlServerDatabaseResource> DropCreateDBCommand(this IResourceBuilder<SqlServerDatabaseResource> db)
    {
        string dbName    = db.Resource.DatabaseName;
        db.WithSqlCommand("dropCreate", $@"
 USE master;
IF DB_ID(N'{dbName}') IS NOT NULL
    BEGIN
        ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE [{dbName}];
    END
CREATE DATABASE [{dbName}];
", ExecCommandEnum.NonQuery,new CommandOptions() { Description = $"Drop and recreate database {dbName}", IconName = "DatabaseWarning" });
        return db;
    }
    static IResourceBuilder<SqlServerDatabaseResource> RecreateWithScripts(this IResourceBuilder<SqlServerDatabaseResource> db, params string[] sqlScripts)
    {
        string sqlAll = string.Join("\r\nGO\r\n", sqlScripts);
        db
            .WithCommand("reset-all", "Reset Everything", async ct =>
            {
            var log = ct.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
            if (log == null) return new ExecuteCommandResult()
            {
                Success = false,
                ErrorMessage = $"no logging available"
            };

            // Get a logger instance for this specific database resource
            var logger = log.GetLogger(db.Resource);
            if (logger == null) return new ExecuteCommandResult()
            {
                Success = false,
                ErrorMessage = $"on {db.Resource.Name} no logger"
            };


            var commandService = ct.ServiceProvider.GetService(typeof(ResourceCommandService)) as ResourceCommandService;
            if (commandService == null) return CommandResults.Failure($"no command service available");

                logger.LogDebug("Starting database system reset...");
            try
            {
                var flushResult = await commandService.ExecuteCommandAsync(db.Resource, "dropCreate");
                var restartResult = await commandService.ExecuteCommandAsync(db.Resource, "Startup_ExecScripts");
                if (!restartResult.Success || !flushResult.Success)
                {
                    return CommandResults.Failure($"System reset failed");
                }

                logger.LogDebug("System reset completed successfully");
                return CommandResults.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "System reset failed");
                return CommandResults.Failure(ex);
            }
        },
new CommandOptions() { Description = $"Drop and recreate database {db.Resource.DatabaseName}", IconName = "ArrowClockwise" })
            ; 
        return db;
    }

}
