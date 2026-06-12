using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using System.Collections;
using System.Text;

Console.WriteLine("Hello, Event Hub!");
string connectionStringEventHubs = "";
foreach (var item in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
{
    if (item.Key?.ToString()?.Contains("azureEventHubs") == true)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
        connectionStringEventHubs = item.Value?.ToString() ?? string.Empty;
    }
}
if (string.IsNullOrWhiteSpace(connectionStringEventHubs ))
{
    Console.WriteLine("Azure Event Hubs connection string is not set in environment variables.");
    return;
}
Console.WriteLine($"Connection string for Azure Event Hubs: {connectionStringEventHubs}");
Console.WriteLine("Setting up the Event Hub producer client...");

EventHubProducerClient producerClient = new(connectionStringEventHubs, "azureEventHubsHub1");
EventData eventData = new EventData("Hello,Andrei Ignat from Azure Event Hubs!");
Console.WriteLine("Sending event to Azure Event Hubs...");
await producerClient.SendAsync([eventData]);


Console.WriteLine("Receiving event from Azure Event Hubs...");
var consumer = new EventHubConsumerClient(EventHubConsumerClient.DefaultConsumerGroupName, connectionStringEventHubs, "azureEventHubsHub1");
await foreach (PartitionEvent partitionEvent in consumer.ReadEventsAsync(new ReadEventOptions { MaximumWaitTime = TimeSpan.FromSeconds(2) }))
{
    if (partitionEvent.Data != null)
    {
        string messageBody = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());
        Console.WriteLine($"!!!!Message received : '{messageBody}'");
    }
    else
    {
        break;
    }
}