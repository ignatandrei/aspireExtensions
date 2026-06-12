using Azure.Identity;
using Azure.Storage.Blobs;
using System.Collections;
using System.Reflection.Metadata;

Console.WriteLine("Hello, AzureStorageClient!");
string connectionStringAzurewStorage = "";
foreach (var item in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
{
    if (item.Key?.ToString()?.Contains("AZURESTORAGEBLOBS1_CONNECTIONSTRING") == true)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
        connectionStringAzurewStorage = item.Value?.ToString() ?? string.Empty;
    }
}
if (string.IsNullOrWhiteSpace(connectionStringAzurewStorage))
{
    Console.WriteLine("Azure Storage connection string is not set in environment variables.");
    return;
}
Console.WriteLine($"Azure storage connection string: {connectionStringAzurewStorage}");
var client = new BlobServiceClient(connectionStringAzurewStorage);
var container= await client.CreateBlobContainerAsync("testcontainer");
Console.WriteLine($"Created container {container.Value.Name}. Please verify in Azure Storage Explorer, https://github.com/microsoft/AzureStorageExplorer/releases, that the container was created successfully.");
