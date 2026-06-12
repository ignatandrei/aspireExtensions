using StackExchange.Redis;
using System.Collections;

Console.WriteLine("Hello, Azure Managed Redis Client!");

string connectionStringAzureManagedRedis = "";
foreach (var item in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
{
    if (item.Key?.ToString()?.Contains("azureManagedRedis") == true)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
        connectionStringAzureManagedRedis = item.Value?.ToString() ?? string.Empty;
    }
}
if (string.IsNullOrWhiteSpace(connectionStringAzureManagedRedis))
{
    Console.WriteLine("Azure Managed Redis connection string is not set in environment variables.");
    return;
}
Console.WriteLine($"Connection string for Azure Managed Redis: {connectionStringAzureManagedRedis}");
var mux = await ConnectionMultiplexer.ConnectAsync(connectionStringAzureManagedRedis);

var db = mux.GetDatabase();
Console.WriteLine("Connected to Azure Managed Redis. Setting and getting a value...");
await db.StringSetAsync("myName", "Andrei");
Console.WriteLine("Set value for 'myName' to 'Andrei' in Azure Managed Redis.");
var value = await db.StringGetAsync("myName");
Console.WriteLine($"Retrieved value for 'myName' from Azure Managed Redis: {value}");
