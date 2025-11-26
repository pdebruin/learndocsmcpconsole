using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
using System.ClientModel;
using OpenAI;

class SkPlugin
{
    public static async Task Run(string[] args)
    {
        // https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Demos/ModelContextProtocolPlugin/Program.cs
        // MCP
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

        // Semantic Kernel
        var endpoint = "https://models.github.ai/inference";
        var credential = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        var model = "openai/gpt-4o-mini";

        if (string.IsNullOrWhiteSpace(credential))
            throw new InvalidOperationException("GITHUB_TOKEN environment variable is not set.");

        var client = new OpenAIClient(new ApiKeyCredential(credential), new OpenAIClientOptions { Endpoint = new Uri(endpoint) });
        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(model, client);
        Kernel kernel = builder.Build();
        kernel.Plugins.AddFromFunctions("LearnMcpPlugin", tools.Select(aiFunction => aiFunction.AsKernelFunction()));

        OpenAIPromptExecutionSettings executionSettings = new()
        {
            Temperature = 0,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true })
        };

        ChatCompletionAgent agent = new()
        {
            Instructions = "Answer questions about Microsoft products, services and technologies",
            Name = "LearnAgent",
            Kernel = kernel,
            Arguments = new KernelArguments(executionSettings),
        };

        var prompt = "How to create an Azure storage account using az cli?";
        await foreach (var content in agent.InvokeStreamingAsync(prompt))
        {
            Console.Write(content.Message);
        }
    }
}