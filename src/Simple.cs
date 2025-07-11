using ModelContextProtocol;
using ModelContextProtocol.Client;

class Simple
{
    public static async Task Main(string[] args)
    {
        System.Console.WriteLine("Running the simple MCP client example...");

        // Create an MCP client using the Stdio transport
        // IMcpClient mcpClient = await McpClientFactory.CreateAsync(
        //     new StdioClientTransport(new()
        //     {
        //         Command = "npx",
        //         Arguments = [
        //       "-y",
        //       "mcp-remote",
        //       "https://learn.microsoft.com/api/mcp"
        //     ],
        //         Name = "Learn Docs MCP Server",
        //     }));

        // Create an MCP client using the HTTP transport
        IMcpClient mcpClient = await McpClientFactory.CreateAsync(
            new SseClientTransport(new()
            {
                Endpoint = new Uri("https://learn.microsoft.com/api/mcp"),
                Name = "Learn Docs MCP Server",
                TransportMode = HttpTransportMode.StreamableHttp,
            }));

        // List all available tools from the MCP server.
        Console.WriteLine("Available tools:");
        IList<McpClientTool> tools = await mcpClient.ListToolsAsync();
        foreach (McpClientTool tool in tools)
        {
            Console.WriteLine($"{tool}");
        }
        Console.WriteLine();

        // Set the specific tool to call
        var toolName = "microsoft_docs_search";

        // Define the arguments for the tool call
        IReadOnlyDictionary<string, object> toolArguments = new Dictionary<string, object>()
        {
            { "question", "how to create an Azure storage account using az cli?" }
        };

        var response = await mcpClient.CallToolAsync(toolName, toolArguments!);

        Console.WriteLine("Response from MCP server:");
        foreach (var res in response.Content)
        {
            Console.WriteLine($"Tool Result: {res.ToAIContent()}");
        }
    }
}