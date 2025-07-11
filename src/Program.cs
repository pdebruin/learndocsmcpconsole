Console.WriteLine("Hello, learndocsmcpconsole. You can choose from the following options:");
Console.WriteLine("1. Run the simple MCP client example");
Console.WriteLine("2. Run the MCP client example with a GitHub model");

string? userinput = Console.ReadLine();

// using ModelContextProtocol;
// using ModelContextProtocol.Client;

switch (userinput)
{
    case "1":
        // Call the method to list available tools
        Simple.Main(args).GetAwaiter().GetResult();
        Console.WriteLine("Listed available tools successfully.");
        break;
    case "2":
        Githubmodel.Main(args).GetAwaiter().GetResult();
        break;
    default:
        Console.WriteLine("Invalid option. Please try again.");
        break;
}