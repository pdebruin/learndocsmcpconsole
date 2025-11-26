Console.WriteLine("Hello, learndocsmcpconsole. You can choose from the following options:");
Console.WriteLine("1. Run the AI client example with GitHub model");
Console.WriteLine("2. Run the SK client example with GitHub model");

string? userinput = Console.ReadLine();

switch (userinput)
{
    case "1":
        AIInference.Run(args).GetAwaiter().GetResult();
        break;
    case "2":
        SkPlugin.Run(args).GetAwaiter().GetResult();
        break;
    default:
        Console.WriteLine("Invalid option. Please try again.");
        break;
}