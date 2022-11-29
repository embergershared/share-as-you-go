// <copyright file="Program.cs" company="PlaceholderCompany">
//
// DISCLAIMER
//
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// </copyright>

using ClientConsoleApp.Classes;
using ClientConsoleApp.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//using Serilog;
//using Serilog.Events;
//using Serilog.Exceptions;
//using Serilog.Sinks.SystemConsole.Themes;

namespace ClientConsoleApp
{
    internal class Program
    {
        private static bool _testPrivateLink = true;
        private static string? _sbNamespace;
        private static string? _sbQueue;
        private static string? _senderClientId;
        private static string? _receiverClientId;

        private static async Task Main(string[] args)
        {
            #region Initialization
            // Configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            // .NET Logging
            using ILoggerFactory loggerFactory =
                LoggerFactory.Create(builder =>
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "HH:mm:ss ";
                    }));

            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

            //// Serilog Logger configuration
            //var outputTemplate =
            //    "[{Timestamp:HH:mm:ss.fff} {Level:u3}] ({SourceContext}.{Method}) {Message:lj}{NewLine}{Exception}";
            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Verbose()
            //    .Enrich.FromLogContext()
            //    .Enrich.WithExceptionDetails()
            //    //.WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
            //    .WriteTo.Console(LogEventLevel.Verbose, outputTemplate, theme: AnsiConsoleTheme.Sixteen)
            //    .WriteTo.AzureAnalytics(workspaceId: config.GetValue<string>("LAW_ID"),
            //        authenticationId: config.GetValue<string>("LAW_KEY"))
            //    .CreateLogger();

            // Services Collection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            //// Get a Logger for Program
            //var logger = serviceProvider.GetService<ILogger<Program>>();

            if (logger == null)
            {
                Console.WriteLine("Could not create a logger. Program execution stopped");
                return;
            }
            #endregion

            // Execution Start
            logger.LogInformation("Starting application");

            // Main program tasks sequences
            SetVariablesValues(logger, config);

            if (!await CheckDnsResolution(logger, serviceProvider)) return;

            await SendMessages(logger, serviceProvider);

            Thread.Sleep(2000);

            //await ReceiveMessage(logger, serviceProvider);

            //await DeleteAllMessages(logger, serviceProvider);

            // End of Main program
            logger.LogInformation("Ending application");
        }

        #region Private Methods
        private static async Task DeleteAllMessages(ILogger<Program> logger, ServiceProvider serviceProvider)
        {
            // Use Queue messaging receiver
            logger.LogDebug("Delete all messages from the Queue");

            var receiver = serviceProvider.GetService<IServiceBusClient>();

            if (receiver != null)
            {
                logger.LogTrace("Got a IServiceBusClient instance");

                if (_sbNamespace != null && receiver.CreateClient(_sbNamespace, _receiverClientId))
                {
                    logger.LogTrace("ServiceBusClient created");

                    if (_sbQueue != null)
                    {
                        await receiver.DeleteAllMessagesAsync(_sbQueue);

                        logger.LogTrace("Disposing client");
                        await receiver.DisposeClientAsync();
                    }
                }
                else
                {
                    logger.LogError("Impossible to create a ServiceBusClient");
                }
            }
            else
            {
                logger.LogError("Impossible to get a IServiceBusClient instance");
            }

            logger.LogDebug("Deleted all messages from the Queue");
        }

        private static async Task ReceiveMessage(ILogger<Program> logger, ServiceProvider serviceProvider)
        {
            // Use Queue messaging receiver
            logger.LogDebug("Receiving the next FIFO message from the Queue");

            var receiver = serviceProvider.GetService<IServiceBusClient>();

            if (receiver != null)
            {
                logger.LogTrace("Got a IServiceBusClient instance");

                if (_sbNamespace != null && receiver.CreateClient(_sbNamespace, _receiverClientId))
                {
                    logger.LogTrace("ServiceBusClient created");

                    // Receiving a message
                    logger.LogTrace("Receiving 1 message from the queue");

                    if (_sbQueue != null)
                    {
                        var receivedMessage = await receiver.ReceiveMessageAsync(_sbQueue);

                        if (receivedMessage != null)
                        {
                            logger.LogInformation("Received message body: {@body}", receivedMessage.Body.ToString());
                        }
                        else
                        {
                            logger.LogWarning("No message received from the queue: {@q_name}", _sbQueue);
                        }

                        logger.LogTrace("Disposing client");
                        await receiver.DisposeClientAsync();
                    }
                }
                else
                {
                    logger.LogError("Impossible to create a ServiceBusClient");
                }
            }
            else
            {
                logger.LogError("Impossible to get a IServiceBusClient instance");
            }

            logger.LogDebug("Received the next FIFO message from the Queue");
        }

        private static async Task SendMessages(ILogger<Program> logger, ServiceProvider serviceProvider)
        {
            // Use Queue messaging sender
            logger.LogDebug("Sending a message to the Service Bus Queue");

            var sender = serviceProvider.GetService<IServiceBusClient>();

            if (sender != null)
            {
                logger.LogTrace("Got a IServiceBusClient instance");

                if (_sbNamespace != null && sender.CreateClient(_sbNamespace, _senderClientId))
                {
                    logger.LogTrace("ServiceBusClient created");

                    // Sending 10 messages
                    // IMPORTANT: There are 2 better ways to send several messages.
                    // Use one of them for production: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet#sending-a-batch-of-messages
                    logger.LogTrace("Sending 10 messages to the queue");

                    for (var i = 1; i < 11; i++)
                    {
                        logger.LogTrace("Sending message #{@num} to the queue", i);

                        var messageContent = $"Message #{i} sent at {DateTime.Now:MM/dd/yyyy hh:mm tt}";

                        var result = _sbQueue != null && await sender.SendMessageAsync(_sbQueue, messageContent);
                        if (result)
                        {
                            logger.LogInformation("Message sent");
                        }

                        // Wait 1 second
                        Thread.Sleep(1000);
                    }

                    logger.LogTrace("10 messages sent to the queue");

                    logger.LogTrace("Disposing client");
                    await sender.DisposeClientAsync();
                }
                else
                {
                    logger.LogError("Impossible to get a ServiceBusClient");
                }
            }
            else
            {
                logger.LogError("Impossible to get a IServiceBusClient instance");
            }

            logger.LogDebug("Sent a message to the Service Bus Queue");
        }

        private static async Task<bool> CheckDnsResolution(ILogger<Program> logger, ServiceProvider serviceProvider)
        {
            // Check DNS resolution to Service Bus
            logger.LogDebug("Launching DNS Resolution");

            var resolver = serviceProvider.GetService<IDnsResolver>();

            if (resolver != null)
            {
                var success = await resolver.ResolveAsync($"{_sbNamespace}.{Constants.SbPublicSuffix}.");
                if (_testPrivateLink)
                {
                    success = success && await resolver.ResolveAsync(
                        $"{_sbNamespace}.{Constants.SbPrivateSuffix}.");
                }

                if (success)
                {
                    logger.LogDebug("DNS Resolution succeeded");
                    return true;
                }
                else
                {
                    logger.LogError("DNS Resolution failed");
                    return false;
                }
            }
            else
            {
                logger.LogError("Impossible to get a DnsResolver");
                return false;
            }
        }

        private static void SetVariablesValues(ILogger<Program> logger, IConfigurationRoot config)
        {
            logger.LogDebug("Getting values from configuration");
            
            if (config.GetChildren().Any(item => item.Key == "TEST_PRIVATE_LINK"))
            {
                _testPrivateLink = config.GetValue<bool>("TEST_PRIVATE_LINK");
            }

            _sbNamespace = config.GetValue<string>("SERVICEBUS_NS_NAME");
            _sbQueue = config.GetValue<string>("QUEUE_NAME");
            _senderClientId = config.GetValue<string>("SENDER_CLIENT_ID");
            _receiverClientId = config.GetValue<string>("RECEIVER_CLIENT_ID");
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Logging
            //serviceCollection
            //    // Serilog logging
            //    .AddLogging(configure => configure.AddSerilog());
            //var serviceProvider = serviceCollection.BuildServiceProvider();
            //var logger = serviceProvider.GetService<ILogger<AnyClass>>();
            //serviceCollection.AddSingleton(typeof(ILogger), logger);


            // Dependency Injection
            serviceCollection
                .AddSingleton<IDnsResolver, DnsResolver>()
                .AddSingleton<IServiceBusClient, ServiceBusClient>();
        }
        #endregion
    }
}