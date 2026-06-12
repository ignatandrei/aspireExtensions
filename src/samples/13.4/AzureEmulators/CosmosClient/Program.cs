using Azure.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using System.Collections;
using System.Net;

Console.WriteLine("Hello, Cosmos Client!");
string connectionStringCosmos = "";
foreach (var item in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
{
    if(item.Key?.ToString()?.Contains("Cosmos") == true)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
        connectionStringCosmos  = item.Value?.ToString() ?? string.Empty;
    }
}
if(string.IsNullOrWhiteSpace(connectionStringCosmos))
{
    Console.WriteLine("Cosmos connection string is not set in environment variables.");
    return;
}
CosmosClientBuilder builder = new CosmosClientBuilder(connectionStringCosmos);
CosmosClient client = new CosmosClient(connectionStringCosmos, new CosmosClientOptions
{
    ConnectionMode = ConnectionMode.Gateway,
    LimitToEndpoint = true
});

Console.WriteLine("CosmosClient created successfully. Attempting to connect to database...");
DatabaseResponse response = await client.CreateDatabaseIfNotExistsAsync(
    id: "CosmosDatabase1"
);  
Database database = response.Database;
Console.WriteLine("Database created successfully. Attempting to connect to container...");
var containerResponse = await database.CreateContainerIfNotExistsAsync(
    id: "CosmosEntriesContainer1",
    partitionKeyPath: "/id"
);
var container = containerResponse.Container;

Console.WriteLine("start write item to CosmosDB");
await container.CreateItemAsync(new { id = Guid.NewGuid().ToString(), name = "Andrei Ignat" });
Console.WriteLine("verify with Explorer that I wrote the item to CosmosDB");