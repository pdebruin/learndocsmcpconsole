using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

class StreamChat
{
    public static async Task Run(string[] args)
    {
        // TODO: merge with https://github.com/modelcontextprotocol/csharp-sdk/blob/main/samples/ChatWithTools/Program.cs

        var endpoint = "https://models.github.ai/inference";
        var credential = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        var model = "openai/gpt-4o-mini";

        var openAIOptions = new OpenAIClientOptions()
        {
            Endpoint = new Uri(endpoint)
        };

        var client = new ChatClient(model, new ApiKeyCredential(credential), openAIOptions);

        List<ChatMessage> messages = new List<ChatMessage>()
        {
            new SystemChatMessage("You are a helpful assistant."),
            new UserChatMessage("What is the capital of France?"),
        };

        var requestOptions = new ChatCompletionOptions()
        {
            Temperature = 1.0f,
            TopP = 1.0f,
            MaxOutputTokenCount = 1000
        };

        var response = await client.CompleteChatAsync(messages, requestOptions);
        System.Console.WriteLine(response.Value.Content[0].Text);
    }
}