using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Collections;

Console.WriteLine("Hello, Azure Storage Queues!");
string connectionStringAzureStorage = "";
foreach (var item in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
{
    if (item.Key?.ToString()?.Contains("AzureStorageQueues1_CONNECTIONSTRING", StringComparison.InvariantCultureIgnoreCase) == true)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
        connectionStringAzureStorage = item.Value?.ToString() ?? string.Empty;
    }
}
if (string.IsNullOrEmpty(connectionStringAzureStorage)){
    Console.WriteLine("no connection string for Azure Storage Queues found in environment variables.");
    return;
}
QueueClient queue = new(connectionStringAzureStorage, "andreiq");
string valueToInsert = "Hello Andrei Ignat !";
await InsertMessageAsync(queue, valueToInsert);
var message= await RetrieveNextMessageAsync(queue); 
Console.WriteLine("the message  is: "+message);
static async Task InsertMessageAsync(QueueClient theQueue, string newMessage)
{
    if (null != await theQueue.CreateIfNotExistsAsync())
    {
        Console.WriteLine($"The queue {theQueue.Name} was created.");
    }

    await theQueue.SendMessageAsync(newMessage);
}

static async Task<string?> RetrieveNextMessageAsync(QueueClient theQueue)
{
    if (await theQueue.ExistsAsync())
    {
        QueueProperties properties = await theQueue.GetPropertiesAsync();

        if (properties.ApproximateMessagesCount > 0)
        {
            QueueMessage[] retrievedMessage = await theQueue.ReceiveMessagesAsync(1);
            string theMessage = retrievedMessage[0].Body.ToString();
            await theQueue.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);
            return theMessage;
        }

        return null;
    }

    return null;
}