using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;

using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

class Openai
{
    public static async Task Run(string[] args)
    {

        // Create an MCPClient for the GitHub server
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

        // Build a kernel
        var endpoint = "https://models.github.ai/inference";
        var credential = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        var model = "openai/gpt-4o-mini";

        var openAIOptions = new OpenAIClientOptions()
        {
            Endpoint = new Uri(endpoint)
        };

       var client = new OpenAIClient(new ApiKeyCredential(credential), openAIOptions);

        // Prepare and build kernel with the MCP tools as Kernel functions
        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(model, client);

        Kernel kernel = builder.Build();

        kernel.Plugins.AddFromFunctions("GitHub", tools.Select(aiFunction => aiFunction.AsKernelFunction()));

        // Enable automatic function calling
        OpenAIPromptExecutionSettings executionSettings = new()
        {
            Temperature = 0,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true })
        };

        // Test using GitHub tools
        var prompt = "how to create an Azure storage account using az cli?";
        var result = await kernel.InvokePromptAsync(prompt, new(executionSettings)).ConfigureAwait(false);
        Console.WriteLine($"\n\n{prompt}\n{result}");

        // // Define the agent
        // ChatCompletionAgent agent = new()
        // {
        //     Instructions = "Answer questions about Microsoft products, services and technologies.",
        //     Name = "LearnAgent",
        //     Kernel = kernel,
        //     Arguments = new KernelArguments(executionSettings),
        // };

        // // Respond to user input, invoking functions where appropriate.
        // ChatMessageContent response = await agent.InvokeAsync("how to create an Azure storage account using az cli?").FirstAsync();
        // Console.WriteLine($"\n\nResponse from LearnAgent:\n{response.Content}");
    }
}