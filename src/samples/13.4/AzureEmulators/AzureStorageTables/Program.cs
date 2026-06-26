using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Data.Tables;
using System.Collections;
using System.Collections.Concurrent;

Console.WriteLine("Hello, AzureStorageClient Tables!");
string connectionStringAzurewStorage = "";
foreach (var item in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
{
    if (item.Key?.ToString()?.Contains("AzureStorageTables1_CONNECTIONSTRING",StringComparison.InvariantCultureIgnoreCase) == true)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
        connectionStringAzurewStorage = item.Value?.ToString() ?? string.Empty;
    }
}

TableClientOptions tce = new ();
tce.AddPolicy(new RemoveAuthorizationHeaderPolicy(), HttpPipelinePosition.PerCall);
TableServiceClient client = new(connectionStringAzurewStorage, tce);
var tableClient = client.GetTableClient("AndreiTable");
await tableClient.CreateIfNotExistsAsync();
var entity = new TableEntity("AndreiPartition", "AndreiRow")
{
    {"FirstName", "Andrei" },
};
await tableClient.UpsertEntityAsync(entity);
var resp = await tableClient.GetEntityAsync<TableEntity>("AndreiPartition", "AndreiRow");
Console.WriteLine($"Retrieved entity: PartitionKey={resp.Value.PartitionKey}, RowKey={resp.Value.RowKey}, FirstName={resp.Value.GetString("FirstName")}");
internal sealed class RemoveAuthorizationHeaderPolicy : HttpPipelinePolicy
{
    public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
    {
        message.Request.Headers.Remove("Authorization");
        ProcessNext(message, pipeline);
    }

    public override ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
    {
        message.Request.Headers.Remove("Authorization");
        return ProcessNextAsync(message, pipeline);
    }
}