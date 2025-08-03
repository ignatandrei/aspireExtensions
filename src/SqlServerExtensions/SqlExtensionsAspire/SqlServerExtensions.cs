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
public  static partial class SqlServerExtensions
{
    /// <summary>
    /// Adds a custom SQL command to a SQL Server database resource that can be executed through the Aspire dashboard.
    /// </summary>
    /// <param name="db">The SQL Server database resource builder to extend</param>
    /// <param name="name">The name of the command as it will appear in the dashboard</param>
    /// <param name="sql">The SQL script to execute when the command is invoked</param>
    /// <param name="commandOptions">Optional command configuration options</param>
    /// <returns>The same resource builder for method chaining</returns>
    public static IResourceBuilder<SqlServerDatabaseResource> WithSqlCommand(this IResourceBuilder<SqlServerDatabaseResource> db, string name, string sql, CommandOptions? commandOptions=null)
    {
        
        db.WithCommand(name, name,async ecc =>
        {
            // Get the resource logger service to log command execution
            var log= ecc.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
            if (log == null) return new ExecuteCommandResult()
            {
                Success = false,
                ErrorMessage = $"no logging available"
            };
            
            // Get a logger instance for this specific database resource
            var lg = log.GetLogger(db.Resource);
            if (log == null) return new ExecuteCommandResult()
            {
                Success = false,
                ErrorMessage = $"on {db.Resource.Name} no logger"
            };
            
            // Retrieve the connection string for the database
            var cn = await db.Resource.ConnectionStringExpression.GetValueAsync(CancellationToken.None);
            if (cn == null) return new ExecuteCommandResult()
            {
                Success = false,
                ErrorMessage = $"no connection available on {db.Resource?.Name}"
            };
            
            // Create and open SQL connection
            using var sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = cn;
            await sqlConnection.OpenAsync(CancellationToken.None);
            
            // Prepare and execute the SQL command
            using var command = sqlConnection.CreateCommand();
            command.CommandText = sql;
            lg.LogInformation("executing : " + sql);
            
            // Set command timeout to 2 minutes (TODO: make this configurable)
            command.CommandTimeout = 120;
            try
            {
                // Execute the SQL command
                await command.ExecuteNonQueryAsync();
                return new ExecuteCommandResult() { Success = true };
            }
            catch(Exception ex)
            {
                // Return error details if execution fails
                return new ExecuteCommandResult() { Success = false,ErrorMessage =ex.Message};

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
    public static IResourceBuilder<SqlServerDatabaseResource> ExecuteSqlServerScripts(this IResourceBuilder<SqlServerDatabaseResource> db, params IEnumerable<string> sqlScripts)
    {
        var builder = db.ApplicationBuilder;

        // Subscribe to the resource ready event to execute scripts when database is available
        db.OnResourceReady(async (dbRes, ev, ct) =>
        {
            // Get the database connection string
            var cn = await dbRes.ConnectionStringExpression.GetValueAsync(ct);
            if (cn == null) return;
            
            // Set up logging for script execution
            var log = ev.Services.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
            if(log == null)
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
            using var sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = cn;
            await sqlConnection.OpenAsync(ct);
            
            int nr = 0;
            // Process each SQL script
            foreach (var item in sqlScripts)
            {
                lg.LogInformation($"Executing script {++nr}");
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

                        // Execute the batch the specified number of times (default: 1)
                        for (var i = 0; i < count; i++)
                        {
                            using var command = sqlConnection.CreateCommand();
                            command.CommandText = batch;
                            // Set command timeout to 2 minutes (TODO: make this configurable)
                            command.CommandTimeout = 120;
                            try
                            {
                                await command.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                // Log errors but continue with remaining scripts
                                lg.LogError(ex,$"!!!!Error in executing {batch} :\r\n !!!! {ex.Message}");
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
                    using var command = sqlConnection.CreateCommand();
                    var batch = batchBuilder.ToString();
                    command.CommandText = batch;
                    // Set command timeout (TODO: make this configurable)
                    try
                    {
                        await command.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        // Log errors for the final batch
                        lg.LogError(ex, $"!!!!Error in executing {batch} :\r\n !!!! {ex.Message}");
                    }
                }
            }
            lg.LogInformation($"Executed {nr} scripts on database {dbRes.Name}");
        });
        return db;
    }
}
