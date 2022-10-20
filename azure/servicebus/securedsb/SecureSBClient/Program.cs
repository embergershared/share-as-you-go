// See https://aka.ms/new-console-template for more information

using Azure.Identity;
using Azure.Messaging.ServiceBus;
using DnsClient;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace SecureSBClient;

internal class Program
{
    private const string Tab = "   ";
    private const string Header = "##################";

    private static async Task Main(string[] args)
    {

        #region Initializing Services
        // Loading settings
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        var config = configBuilder.Build();

        // Creating a Logger (based on SeriLog)
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        // Trace Program start
        TraceProgramStart(args);

        // Trace settings if in Debug
        TraceAppSettings(config, "Logging");
        TraceAppSettings(config, "ConnectionStrings");
        TraceAppSettings(config, "PhotosOrg");
        #endregion

        const string sbNamespace = "emberger-poc";
        const string queueName = "queue1";
        const string userAssignedClientId = "70c382c9-af8d-4aae-9167-e20d26d7f42a"; // This is the Client ID of the User Assigned Managed Identity

        Log.Information("{ProgName} program started with no arguments", "SecureSBClient.dll");

        Console.WriteLine($"{Tab}{Header} Part1: Let's resolve some FQDNs");
        Console.WriteLine();

        await DnsResolver($"{sbNamespace}.servicebus.windows.net.");
        Console.WriteLine();
        await DnsResolver($"{sbNamespace}.privatelink.servicebus.windows.net.");

        Console.WriteLine();
        Console.WriteLine($"{Tab}Part1 finished.");

        Console.WriteLine();
        Console.WriteLine($"{Tab}{Header} Part2: Let's send messages to a queue");

        // Reference for the ServiceBusClient: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet
        // Reference for authentication with Azure.Identity: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet#authenticating-with-azureidentity

        // Create a ServiceBusClient that will authenticate through Active Directory
        Console.WriteLine($"{Tab}{Tab}Creating a ServiceBusClient to the Service Bus namespace: {sbNamespace}.servicebus.windows.net");
        string fullyQualifiedNamespace = $"{sbNamespace}.servicebus.windows.net";

        // Code for system-assigned managed identity:
        //ServiceBusClient client = new ServiceBusClient(fullyQualifiedNamespace, new DefaultAzureCredential());

        // Code for user-assigned managed identity:
        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId });
        ServiceBusClient client = new(fullyQualifiedNamespace, credential);

        // Send messages to the queue
        // create the sender
        Console.WriteLine($"{Tab}{Tab}Creating a ServiceBusSender for the queue: {queueName}");
        ServiceBusSender sender = client.CreateSender(queueName);

        // create a message that we can send. UTF-8 encoding is used when providing a string.
        var messageContent = $"Hello world at {DateTime.Now:MM/dd/yyyy hh:mm tt}";
        Console.WriteLine($"{Tab}{Tab}Creating the message with content: {messageContent}");
        ServiceBusMessage message = new(messageContent);

        // send the message
        try
        {
            Console.WriteLine($"{Tab}{Tab}Sending the message to the queue");

            await sender.SendMessageAsync(message);

            Console.WriteLine($"{Tab}{Tab}Message sent to the queue");
        }
        catch (Exception e)
        {
            Console.WriteLine($"{Tab}{Tab}Exception occurred when sending the message: {e}");

            Console.WriteLine($"{Tab}{Tab}Message failed to be sent to the queue");
        }

        Console.WriteLine();
        Console.WriteLine($"{Tab}Part2 finished.");

        Log.Information("{ProgName} program finished its execution", "SecureSBClient.dll");
    }

    #region Static Classes
    private static void TraceProgramStart(string[] args)
    {
        if (args.Length == 0)
        {
            Log.Information("{ProgName} program started with no arguments", "SecureSBClient.dll");
        }
        else
        {
            Log.Information("{ProgName} program started with arguments: {Args}", "SecureSBClient.dll", args);
        }
    }

    private static void TraceAppSettings(IConfigurationRoot config, string section)
    {
        Log.Information("Tracing Settings for section: {Section}", section);
        var loggingSettings = config.GetSection(section).AsEnumerable();

        foreach (var setting in loggingSettings)
        {
            if (string.IsNullOrEmpty(setting.Value))
            {
                Log.Information("      The key: {Key} has no value", setting.Key);
            }
            else
            {
                Log.Information("      The key: {Key} has the value: {Value}", setting.Key, setting.Value);
            }
        }
    }

    private static async Task DnsResolver(string fqdn)
    {
        Console.WriteLine($"Resolving: {fqdn}");
        var lookup = new LookupClient();
        var result = await lookup.QueryAsync(fqdn, QueryType.A);

        if (result.HasError)
        {
            Console.WriteLine($"{Tab}Impossible to resolve {fqdn}");
        }
        else
        {
            Console.WriteLine($"{Tab}Results from DNS Server: {result.NameServer}");
            foreach (var nsRecord in result.Answers)
            {
                Console.WriteLine($"{Tab}{Tab}{nsRecord.ToString()}");
            }
        }
    }
    #endregion
}