using Azure.AI.Projects;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Collections;

Console.WriteLine("Hello, foundry!");
string connectionStringFoundry = "";
foreach (var item in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>())
{
    if (item.Key?.ToString()?.Contains("foundry") == true)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
        connectionStringFoundry = item.Value?.ToString() ?? string.Empty;
    }
}
if (string.IsNullOrWhiteSpace(connectionStringFoundry))
{
    Console.WriteLine("Foundry connection string is not set in environment variables.");
    return;
}

//AIProjectClient projectClient =new ("OPENAI_API_KEY");
string? endpoint = Environment.GetEnvironmentVariable("FOUNDRYCHAT_URI");
string? apiKey = Environment.GetEnvironmentVariable("FOUNDRYCHAT_KEY");
string? modelId = Environment.GetEnvironmentVariable("FOUNDRYCHAT_MODELNAME");
if(string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(modelId))
{
    Console.WriteLine($"Please set the environment variables FOUNDRYCHAT_URI, FOUNDRYCHAT_KEY and FOUNDRYCHAT_MODELNAME before running the application.");
    return;
}
ApiKeyCredential credential = new ApiKeyCredential(apiKey!);
OpenAIClientOptions clientOptions = new OpenAIClientOptions
{
    Endpoint = new Uri(endpoint!)
};
OpenAIClient client = new OpenAIClient(credential, clientOptions);
ChatClient chatClient = client.GetChatClient(modelId!);
string question = "Hello! What do you know about Andrei Ignat?";
Console.WriteLine($"Asking question: {question}");
var completionUpdates = chatClient.CompleteChatStreamingAsync(question);

Console.WriteLine($"[Model {modelId} answer ]: ");
await foreach (var update in completionUpdates)
{
    if (update.ContentUpdate.Count > 0)
    {
        Console.Write(update.ContentUpdate[0].Text);
    }
}
Console.WriteLine();
Console.WriteLine($"end [Model {modelId} answer ]: ");