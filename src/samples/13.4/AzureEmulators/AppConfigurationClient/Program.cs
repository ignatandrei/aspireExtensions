using Azure.Data.AppConfiguration;
using System.Collections;

Console.WriteLine("Hello,AzureAppConfiguration!");


string connectionStringAzureAppConfig = "";
foreach (var item in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
{
    if (item.Key?.ToString()?.Contains("azureAppConfig") == true)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
        connectionStringAzureAppConfig = item.Value?.ToString() ?? string.Empty;
    }
}
if (string.IsNullOrWhiteSpace(connectionStringAzureAppConfig))
{
    Console.WriteLine("Azure App Configuration connection string is not set in environment variables.");
    return;
}
Console.WriteLine($"Connection string for Azure App Configuration: {connectionStringAzureAppConfig}");
var client = new ConfigurationClient(connectionStringAzureAppConfig);
await client.SetConfigurationSettingAsync("Name","Andrei");
Console.WriteLine("Set configuration setting 'Name' to 'Andrei' in Azure App Configuration.");
var name = await client.GetConfigurationSettingAsync("Name");
Console.WriteLine($"Retrieved configuration setting 'Name' from Azure App Configuration: {name.Value}");