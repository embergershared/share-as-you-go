﻿// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

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
            #region Initialization
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

            // Get a Logger for Program
            var logger = serviceProvider.GetService<ILogger<Program>>();

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

            await CheckDnsResolution(logger, serviceProvider);

            await SendMessages(logger, serviceProvider);

            Thread.Sleep(2000);

            await ReceiveMessages(logger, serviceProvider);

            // End of Main program
            logger.LogInformation("Ending application");
        }

        #region Private Methods
        private static async Task ReceiveMessages(ILogger<Program> logger, ServiceProvider serviceProvider)
        {
            // Use Queue messaging receiver
            logger.LogDebug("Launching a Queue Message receiver");

            var receiver = serviceProvider.GetService<IServiceBusClient>();

            if (receiver != null)
            {
                logger.LogTrace("Got a IServiceBusClient instance");

                if (_sbNamespace != null && receiver.CreateClient(_sbNamespace, _receiverClientId))
                {
                    logger.LogTrace("ServiceBusClient created");

                    // Sending a message
                    logger.LogTrace("Receiving all messages from the queue");


                    logger.LogTrace("Received all messages from the queue");

                    await receiver.DisposeClientAsync();
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

            logger.LogDebug("Queue Message receiver finished");
        }

        private static async Task SendMessages(ILogger<Program> logger, ServiceProvider serviceProvider)
        {
            // Use Queue messaging sender
            logger.LogDebug("Launching a Queue Message sender");

            var sender = serviceProvider.GetService<IServiceBusClient>();

            if (sender != null)
            {
                logger.LogTrace("Got a IServiceBusClient instance");

                if (_sbNamespace != null && sender.CreateClient(_sbNamespace, _senderClientId))
                {
                    logger.LogTrace("ServiceBusClient created");

                    // Sending a message
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

            logger.LogDebug("Queue Message sender finished");
        }

        private static async Task CheckDnsResolution(ILogger<Program> logger, ServiceProvider serviceProvider)
        {
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
        }

        private static void SetVariablesValues(ILogger<Program> logger, IConfigurationRoot config)
        {
            logger.LogDebug("Getting values from configuration");
            _sbNamespace = config.GetValue<string>("SERVICEBUS_NS_NAME");
            _sbQueue = config.GetValue<string>("QUEUE_NAME");
            _senderClientId = config.GetValue<string>("SENDER_CLIENT_ID");
            _receiverClientId = config.GetValue<string>("RECEIVER_CLIENT_ID");
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
                .AddSingleton<IServiceBusClient, ServiceBusClient>();
        }
        #endregion
    }
}