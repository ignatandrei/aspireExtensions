using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlExtensionsAspire;

public  static partial class SqlServerExtensions
{
    public static IResourceBuilder<SqlServerDatabaseResource> WithSqlCommand(this IResourceBuilder<SqlServerDatabaseResource> db, string name, string sql, CommandOptions? commandOptions=null)
    {
        
        db.WithCommand(name, name,async ecc =>
        {
            var log= ecc.ServiceProvider.GetService(typeof(ResourceLoggerService)) as ResourceLoggerService;
            if (log == null) return new ExecuteCommandResult()
            {
                Success = false,
                ErrorMessage = $"no logging available"
            };
            var lg = log.GetLogger(db.Resource);
            if (log == null) return new ExecuteCommandResult()
            {
                Success = false,
                ErrorMessage = $"on {db.Resource.Name} no logger"
            };
            var cn = await db.Resource.ConnectionStringExpression.GetValueAsync(CancellationToken.None);
            if (cn == null) return new ExecuteCommandResult()
            {
                Success = false,
                ErrorMessage = $"no connection available on {db.Resource?.Name}"
            };
            using var sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = cn;
            await sqlConnection.OpenAsync(CancellationToken.None);
            using var command = sqlConnection.CreateCommand();
            command.CommandText = sql;
            lg.LogInformation("executing : " + sql);
            //TODO: modify the timeout
            command.CommandTimeout = 120;
            try
            {
                await command.ExecuteNonQueryAsync();
                return new ExecuteCommandResult() { Success = true };
            }
            catch(Exception ex)
            {
                return new ExecuteCommandResult() { Success = false,ErrorMessage =ex.Message};

            }
        }, commandOptions);
        return db;
    }
    public static IResourceBuilder<SqlServerDatabaseResource> WithSqlPadViewerForDB(this IResourceBuilder<SqlServerDatabaseResource> db, IResourceBuilder<SqlServerServerResource> sqlserver)
    {
        var builder = db.ApplicationBuilder;

        var sqlpad = builder
    .AddContainer("sqlpad", "sqlpad/sqlpad:latest")
    .WithEndpoint(5600, 3000, "http")
    .WithEnvironment("SQLPAD_AUTH_DISABLED", "true")
    .WithEnvironment("SQLPAD_AUTH_DISABLED_DEFAULT_ROLE", "Admin")
    .WithEnvironment("SQLPAD_ADMIN", "admin@sqlpad.com")

    .WithEnvironment("SQLPAD_ADMIN_PASSWORD", "admin")
    .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo__name", sqlserver.Resource.Name)
    .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo__driver", "sqlserver")
    .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo__host", sqlserver.Resource.Name)
    .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo__database", db.Resource.Name)
    .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo__username", "sa")
    .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo__password", sqlserver.Resource.PasswordParameter.Value)


    .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo1__name", "SqlMaster")
    .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo1__driver", "sqlserver")
    .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo1__host", sqlserver.Resource.Name)
    .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo1__database", "master")
    .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo1__username", "sa")
    .WithEnvironment("SQLPAD_CONNECTIONS__sqlserverdemo1__password", sqlserver.Resource.PasswordParameter.Value)
    .WithParentRelationship(db)
    .WaitFor(db)
    .WaitFor(sqlserver)
    ;
        return db;

    }


    // https://learn.microsoft.com/sql/t-sql/language-elements/sql-server-utilities-statements-go
    [GeneratedRegex(@"^\s*GO(?<repeat>\s+\d{1,6})?(\s*\-{2,}.*)?\s*$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    internal static partial Regex GoStatements();

    public static IResourceBuilder<SqlServerDatabaseResource> ExecuteSqlServerScripts(this IResourceBuilder<SqlServerDatabaseResource> db, params IEnumerable<string> sqlScripts)
    {
        var builder = db.ApplicationBuilder;
        builder.Eventing.Subscribe<ResourceReadyEvent>(async (ev, ct) =>
        {
            if (ev.Resource is not SqlServerDatabaseResource dbRes) return;
            if (db.Resource != dbRes) return;
            var cn = await dbRes.ConnectionStringExpression.GetValueAsync(ct);
            if (cn == null) return;
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
            using var sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = cn;
            await sqlConnection.OpenAsync(ct);
            int nr = 0;
            foreach (var item in sqlScripts)
            {
                lg.LogInformation($"Executing script {++nr}");
                using var reader = new StringReader(item);
                var batchBuilder = new StringBuilder();

                while (reader.ReadLine() is { } line)
                {
                    var matchGo = GoStatements().Match(line);

                    if (matchGo.Success)
                    {
                        // Execute the current batch
                        var count = matchGo.Groups["repeat"].Success ? int.Parse(matchGo.Groups["repeat"].Value, CultureInfo.InvariantCulture) : 1;
                        var batch = batchBuilder.ToString();

                        for (var i = 0; i < count; i++)
                        {
                            using var command = sqlConnection.CreateCommand();
                            command.CommandText = batch;
                            //TODO: modify the timeout
                            command.CommandTimeout = 120;
                            try
                            {
                                await command.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                //TODO: log
                                lg.LogError(ex,$"!!!!Error in executing {batch} :\r\n !!!! {ex.Message}");
                            }
                        }

                        batchBuilder.Clear();
                    }
                    else
                    {
                        // Prevent batches with only whitespace
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            batchBuilder.AppendLine(line);
                        }
                    }
                }

                // Process the remaining batch lines
                if (batchBuilder.Length > 0)
                {
                    using var command = sqlConnection.CreateCommand();
                    var batch = batchBuilder.ToString();
                    command.CommandText = batch;
                    //TODO: modify the timeout
                    try
                    {
                        await command.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        //TODO: log

                        lg.LogError(ex, $"!!!!Error in executing {batch} :\r\n !!!! {ex.Message}");
                    }
                }
            }
            lg.LogInformation($"Executed {nr} scripts on database {dbRes.Name}");
        });
        return db;
    }
}
