using Azure.Messaging.ServiceBus;
using System.Collections;
using System.Diagnostics;

Console.WriteLine("Hello,AzureServiceBus!");


string connectionStringAzureServiceBus = "";
foreach (var item in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
{
    if (item.Key?.ToString()?.Contains("azureServiceBus1") == true)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
        connectionStringAzureServiceBus = item.Value?.ToString() ?? string.Empty;
    }
}
if (string.IsNullOrWhiteSpace(connectionStringAzureServiceBus))
{
    Console.WriteLine("AzureServiceBus connection string is not set in environment variables.");
    return;
}
Console.WriteLine($"Connection string for Azure SQL: {connectionStringAzureServiceBus}");


ServiceBusClient client=new ServiceBusClient(connectionStringAzureServiceBus);


Console.WriteLine("Setting up the Service Bus processor...");
ServiceBusProcessor processor = client.CreateProcessor("azureServiceQueue1", new ServiceBusProcessorOptions());
processor.ProcessMessageAsync += (ProcessMessageEventArgs arg) =>
{
    Console.WriteLine("!!!Received message: " + arg.Message.Body.ToString());
    return Task.CompletedTask;
};
processor.ProcessErrorAsync += (ProcessErrorEventArgs arg) =>
{
    Console.WriteLine("Error processing message: " + arg.Exception.ToString());
    return Task.CompletedTask;
};
//do not put await here
processor.StartProcessingAsync();
Console.WriteLine("Starting the Service Bus sender...");
ServiceBusSender sender = client.CreateSender("azureServiceQueue1");
ServiceBusMessage message = new ServiceBusMessage("Hello, Andrei Ignat from Azure Service Bus!");

await sender.SendMessageAsync(message);
await Task.Delay(60_000);
await sender.DisposeAsync();
await client.DisposeAsync();
