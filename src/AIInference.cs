using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using System.ClientModel;

class AIInference
{
    public static async Task Run(string[] args)
    {
        // https://www.nuget.org/packages/Microsoft.Extensions.AI.AzureAIInference/
        // https://github.com/modelcontextprotocol/csharp-sdk/blob/main/samples/ChatWithTools/Program.cs
        IMcpClient mcpClient = await McpClientFactory.CreateAsync(
            new SseClientTransport(new()
            {
                Endpoint = new Uri("https://learn.microsoft.com/api/mcp"),
                Name = "Learn Docs MCP Server",
                TransportMode = HttpTransportMode.StreamableHttp,
            }));

        var tools = await mcpClient.ListToolsAsync().ConfigureAwait(false);
        foreach (var tool in tools)
        {
            Console.WriteLine($"{tool.Name}");
        }

        var endpoint = "https://models.github.ai/inference";
        var credential = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        var model = "openai/gpt-4o-mini";

        IChatClient client = new Azure.AI.Inference.ChatCompletionsClient(
            new(endpoint),
            new AzureKeyCredential(credential))
            .AsIChatClient(model);

        IChatClient chatClient = new ChatClientBuilder(client)
        .UseFunctionInvocation()
        .Build();

        ChatOptions chatOptions = new()
        {
            Tools = [.. tools]
        };

        await foreach (var message in chatClient.GetStreamingResponseAsync("How to create an Azure storage account using az cli?", chatOptions))
        {
            Console.Write(message);
        }
    }
}