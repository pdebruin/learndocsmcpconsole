using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using System.ClientModel;
using System.Linq;

class AIInference
{
    public static async Task Run(string[] args)
    {
        // https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-client
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
        
        if (string.IsNullOrEmpty(credential))
            throw new InvalidOperationException("GITHUB_TOKEN environment variable is not set.");

        IChatClient chatClient =
            new ChatClientBuilder(
                new ChatCompletionsClient(new Uri(endpoint),
                new AzureKeyCredential(credential))
                .AsIChatClient(model))
            .UseFunctionInvocation()
            .Build();

        // Summarize tools to avoid sending large tool metadata to the model (token limit).
        var toolNames = string.Join(", ", tools.Select(t => t.Name).Take(50));
        var prompt = $"Available tools: {toolNames}\n\nHow to create an Azure storage account using az cli?";

        var chatOptions = new ChatOptions();

        await foreach (var message in chatClient.GetStreamingResponseAsync(prompt, chatOptions))
        {
            Console.Write(message);
        }
    }
}