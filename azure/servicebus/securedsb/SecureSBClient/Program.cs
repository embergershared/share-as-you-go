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
        //private static string? _receiverClientId;

        private static async Task Main()
        {
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
                //_receiverClientId = config.GetValue<string>("RECEIVER_CLIENT_ID");

                // Check DNS resolution to Service Bus
                logger.LogDebug("Launching DNS Resolution");

                var resolver = serviceProvider.GetService<IDnsResolver>();

                if (resolver != null)
                {
                    await resolver.ResolveAsync($"{_sbNamespace}.{Constants.SbPublicSuffix}.");
                    await resolver.ResolveAsync(
                        $"{_sbNamespace}.{Constants.SbPrivateSuffix}.");
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

                logger.LogDebug("Queue Message sender finished");

                Thread.Sleep(2000);

                // Use Queue messaging receiver
                logger.LogDebug("Launching Queue Message receiver");

                //var receiver = serviceProvider.GetService<IQueueMessageReceiver>();


                logger.LogDebug("Queue Message receiver finished");

                logger.LogInformation("Ending application");
            }
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
    }
}