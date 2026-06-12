
using Microsoft.Data.SqlClient;
using System.Collections;

Console.WriteLine("Hello, Azure Sql Client!");
string connectionStringSqlAzure = "";
foreach (var item in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
{
    if (item.Key?.ToString()?.Contains("AzureSql") == true)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
        connectionStringSqlAzure = item.Value?.ToString() ?? string.Empty;
    }
}
if (string.IsNullOrWhiteSpace(connectionStringSqlAzure))
{
    Console.WriteLine("Cosmos connection string is not set in environment variables.");
    return;
}
Console.WriteLine($"Connection string for Azure SQL: {connectionStringSqlAzure}");
await using var connection = new SqlConnection(connectionStringSqlAzure);
await connection.OpenAsync();
Console.WriteLine($"Success opening the database");
