using Npgsql;
using System.Collections;

Console.WriteLine("Hello, Postgres!");


string connectionStringPostgres = "";
foreach (var item in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
{
    if (item.Key?.ToString()?.Contains("postgres") == true)
    {
        //Console.WriteLine($"{item.Key}: {item.Value}");
        connectionStringPostgres = item.Value?.ToString() ?? string.Empty;
    }
}
if (string.IsNullOrWhiteSpace(connectionStringPostgres))
{
    Console.WriteLine("Postgres  connection string is not set in environment variables.");
    return;
}
Console.WriteLine($"Connection string for Postgres: {connectionStringPostgres}");
await using var conn = new NpgsqlConnection(connectionStringPostgres);
await conn.OpenAsync();
Console.WriteLine($"Success opening the database");