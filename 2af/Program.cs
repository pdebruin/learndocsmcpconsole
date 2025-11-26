using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// See https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/GettingStarted/ModelContextProtocol/FoundryAgent_Hosted_MCP/Program.cs

var endpoint = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_ENDPOINT") ?? throw new InvalidOperationException("AZURE_FOUNDRY_PROJECT_ENDPOINT is not set.");
var model = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";

// Get a client to create/retrieve server side agents with.
var persistentAgentsClient = new PersistentAgentsClient(endpoint, new AzureCliCredential());


// Create an MCP tool definition that the agent can use.
var mcpTool = new HostedMcpServerTool(
    serverName: "microsoft_learn",
    serverAddress: "https://learn.microsoft.com/api/mcp")
{
    AllowedTools = ["microsoft_docs_search"],
    ApprovalMode = HostedMcpServerToolApprovalMode.NeverRequire
};

// Create a server side persistent agent with the mcp tool, and expose it as an AIAgent.
AIAgent agent = await persistentAgentsClient.CreateAIAgentAsync(
    model: model,
    options: new()
    {
        Name = "MicrosoftLearnAgent",
        Instructions = "You answer questions by searching Microsoft Learn content",
        ChatOptions = new()
        {
            Tools = [mcpTool]
        },
    });

// You can then invoke the agent like any other AIAgent.
AgentThread thread = agent.GetNewThread();
Console.WriteLine(await agent.RunAsync("How to create a Foundry instance using az cli?", thread));

// Cleanup for sample purposes.
await persistentAgentsClient.Administration.DeleteAgentAsync(agent.Id);