// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecureSBClient.Classes;
using SecureSBClient.Interfaces;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;

namespace SecureSBClient
{
    internal class Program
    {
        private static string? _sbNamespace;
        private static string? _sbQueue;
        private static string? _senderClientId;
        private static string? _receiverClientId;

        private static async Task Main()
        {
            #region ConsoleApp

            // Configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Serilog Logger configuration
            var outputTemplate =
                "[{Timestamp:HH:mm:ss.fff} {Level:u3}] ({SourceContext}.{Method}) {Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                //.WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
                .WriteTo.Console(LogEventLevel.Verbose, outputTemplate, theme: AnsiConsoleTheme.Sixteen)
                .CreateLogger();

            // Services Collection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Start execution
            var logger = serviceProvider.GetService<ILogger<Program>>();

            if (logger != null)
            {
                logger.LogInformation("Starting application");

                logger.LogDebug("Getting values from configuration");
                _sbNamespace = config.GetValue<string>("SERVICEBUS_NS_NAME");
                _sbQueue = config.GetValue<string>("QUEUE_NAME"); 
                _senderClientId = config.GetValue<string>("SENDER_CLIENT_ID");
                _receiverClientId = config.GetValue<string>("RECEIVER_CLIENT_ID");

                // Check DNS resolution to Service Bus
                logger.LogDebug("Launching DNS Resolution");

                var resolver = serviceProvider.GetService<IDnsResolver>();

                if (resolver != null)
                {
                    await resolver.ResolveAsync($"{_sbNamespace}.{Constants.sbPublicSuffix}.");
                    await resolver.ResolveAsync(
                        $"{_sbNamespace}.{Constants.sbPrivateSuffix}.");
                }
                else
                {
                    logger.LogError("Impossible to get a DnsResolver");
                }

                logger.LogDebug("DNS Resolution done");

                // Use Queue messaging sender
                logger.LogDebug("Launching Queue Message sender");

                var sender = serviceProvider.GetService<IQueueMessageSender>();

                if (sender != null)
                {
                    logger.LogTrace("Got a IQueueMessageSender instance");

                    if (sender.CreateClient(_sbNamespace, _senderClientId))
                    {
                        logger.LogTrace("Got a ServiceBus Client");

                        // We have a client to ServiceBus

                        // Sending a message
                        logger.LogTrace("Sending 10 messages to the queue");

                        for (var i = 1; i < 11; i++)
                        {
                            logger.LogTrace("Sending message #{@num} to the queue", i);

                            var messageContent = $"Message #{i} sent at {DateTime.Now:MM/dd/yyyy hh:mm tt}";

                            var result = await sender.SendMessageAsync(_sbQueue, messageContent);
                            if (result)
                            {
                                logger.LogInformation("Message sent");
                            }

                            // Wait 1 second
                            Thread.Sleep(1000);
                        }

                        logger.LogTrace("10 messages sent to the queue");
                    }
                    else
                    {
                        logger.LogError("Impossible to get a ServiceBusClient");
                    }
                }
                else
                {
                    logger.LogError("Impossible to get a QueueMessageSender");
                }

                // Use Queue messaging receiver
                //var receiver = serviceProvider.GetService<IQueueMessageReceiver>();


                logger.LogInformation("Ending application");
            }

            #endregion

            #region InitialCode

            /*
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
            TraceAppSettings(config, "SecureSBClient");
            #endregion
    
            const string sbNamespace = "emberger-poc"; // SERVICEBUS_NS_NAME
            const string queueName = "queue1"; // QUEUE_NAME
            const string userAssignedClientId = "70c382c9-af8d-4aae-9167-e20d26d7f42a"; // This is the Client ID of the User Assigned Managed Identity
    
            // SENDER_CLIENT_ID
            // RECEIVER_CLIENT_ID

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
            var fullyQualifiedNamespace = $"{sbNamespace}.servicebus.windows.net";
    
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
            */

            #endregion
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Logging
            serviceCollection
                // Serilog logging
                .AddLogging(configure => configure.AddSerilog());

            // Dependency Injection
            serviceCollection
                .AddSingleton<IDnsResolver, DnsResolver>()
                .AddSingleton<IQueueMessageSender, QueueMessageSender>();
        }

        #region Static Classes
        /*
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
        */
        #endregion
    }
}