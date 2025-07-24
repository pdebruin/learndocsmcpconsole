Console.WriteLine("Hello, learndocsmcpconsole. You can choose from the following options:");
Console.WriteLine("1. Run the simple MCP client example");
Console.WriteLine("2. Run the MCP client example with a GitHub model");
Console.WriteLine("3. Run the SK client example with GitHub model");

string? userinput = Console.ReadLine();

switch (userinput)
{
    case "1":
        // Call the method to list available tools
        CallTool.Run(args).GetAwaiter().GetResult();
        break;
    case "2":
        AIInference.Run(args).GetAwaiter().GetResult();
        break;
    case "3":
        SkPlugin.Run(args).GetAwaiter().GetResult();
        break;
    default:
        Console.WriteLine("Invalid option. Please try again.");
        break;
}