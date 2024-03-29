﻿// <copyright file="Console.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using ClientConsoleAppV2.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClientConsoleAppV2.Classes
{
    internal class Console : IConsole
    {
        private readonly ILogger<Console> _logger;
        private readonly IConfiguration _config;
        private readonly IDnsResolver _dnsResolver;
        private readonly IServiceBusClient _serviceBusClient;

        private static bool _testPrivateLink = true;
        private static string? _sbNamespace;
        private static string? _sbQueue;
        private static string? _senderClientId;
        private static string? _receiverClientId;


        public Console(
            ILogger<Console> logger,
            IConfiguration config,
            IDnsResolver dnsResolver,
            IServiceBusClient serviceBusClient
            )
        {
            _logger = logger;
            _config = config;
            _dnsResolver = dnsResolver;
            _serviceBusClient = serviceBusClient;
        }

        public async Task ExecuteAsync()
        {
            using (_logger.BeginScope("ExecuteAsync()"))
            {
                _logger.LogTrace("Method start");

                GetVarValues();

                if (!await CheckDnsResolution()) return;

                await SendMessages();

                Thread.Sleep(2000);

                await ReceiveMessage();

                await DeleteAllMessages();

                _logger.LogTrace("Method end");
            }
        }

        #region Private Methods
        private void GetVarValues()
        {
            using (_logger.BeginScope("GetVarValues()"))
            {
                _logger.LogTrace("Method start");

                if (_config.GetChildren().Any(item => item.Key == "TEST_PRIVATE_LINK"))
                {
                    _testPrivateLink = _config.GetValue<bool>("TEST_PRIVATE_LINK");
                    _logger.LogDebug("TEST_PRIVATE_LINK: {v}", _testPrivateLink);
                }

                _sbNamespace = _config.GetValue<string>("SERVICEBUS_NS_NAME");
                _sbQueue = _config.GetValue<string>("QUEUE_NAME");
                _senderClientId = _config.GetValue<string>("SENDER_CLIENT_ID");
                _receiverClientId = _config.GetValue<string>("RECEIVER_CLIENT_ID");

                _logger.LogDebug("SERVICEBUS_NS_NAME: {v}", _sbNamespace);
                _logger.LogDebug("QUEUE_NAME: {v}", _sbQueue);
                _logger.LogDebug("SENDER_CLIENT_ID: {v}", _senderClientId);
                _logger.LogDebug("RECEIVER_CLIENT_ID: {v}", _receiverClientId);

                _logger.LogTrace("Method end");
            }
        }

        private async Task<bool> CheckDnsResolution()
        {
            using (_logger.BeginScope("CheckDnsResolution()"))
            {
                _logger.LogTrace("Method start");

                var result = false;

                // Check DNS resolution to Service Bus
                _logger.LogDebug("Launching DNS Resolution");

                var success = await _dnsResolver.ResolveAsync($"{_sbNamespace}.{Constants.SbPublicSuffix}.");
                if (_testPrivateLink)
                {
                    success = success && await _dnsResolver.ResolveAsync(
                        $"{_sbNamespace}.{Constants.SbPrivateSuffix}.");
                }

                if (success)
                {
                    _logger.LogDebug("DNS Resolution succeeded");
                    result = true;
                }
                else
                {
                    _logger.LogError("DNS Resolution failed");
                }

                _logger.LogTrace("Method end");
                return result;
            }
        }

        private async Task SendMessages()
        {
            using (_logger.BeginScope("SendMessages()"))
            {
                _logger.LogTrace("Method start");

                // Use Queue messaging sender
                _logger.LogTrace("Got a IServiceBusClient instance");

                if (_sbNamespace != null && _serviceBusClient.CreateClient(_sbNamespace, _senderClientId))
                {
                    _logger.LogTrace("ServiceBusClient created");

                    // Sending 10 messages
                    // IMPORTANT: There are 2 better ways to send several messages.
                    // TODO: Use one of them for production: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet#sending-a-batch-of-messages
                    _logger.LogTrace("Sending 10 messages to the queue");

                    for (var i = 1; i < 11; i++)
                    {
                        _logger.LogTrace("Sending message #{@num} to the queue", i);

                        var messageContent = $"Message #{i} sent at {DateTime.Now:MM/dd/yyyy hh:mm tt}";

                        var result = _sbQueue != null && await _serviceBusClient.SendMessageAsync(_sbQueue, messageContent);
                        if (result)
                        {
                            _logger.LogInformation("Message sent");
                        }

                        // Wait 1 second
                        Thread.Sleep(1000);
                    }

                    _logger.LogTrace("10 messages sent to the queue");

                    _logger.LogTrace("Disposing client");
                    await _serviceBusClient.DisposeClientAsync();
                }
                else
                {
                    _logger.LogError("Impossible to get a ServiceBusClient");
                }

                _logger.LogDebug("Sent a message to the Service Bus Queue");

                _logger.LogTrace("Method end");
            }
        }

        private async Task ReceiveMessage()
        {
            using (_logger.BeginScope("ReceiveMessage()"))
            {
                _logger.LogTrace("Method start");

                // Use Queue messaging receiver
                _logger.LogDebug("Receiving the next FIFO message from the Queue");

                _logger.LogTrace("Got a IServiceBusClient instance");

                if (_sbNamespace != null && _serviceBusClient.CreateClient(_sbNamespace, _receiverClientId))
                {
                    _logger.LogTrace("ServiceBusClient created");

                    // Receiving a message
                    _logger.LogTrace("Receiving 1 message from the queue");

                    if (_sbQueue != null)
                    {
                        var receivedMessage = await _serviceBusClient.ReceiveMessageAsync(_sbQueue);

                        if (receivedMessage != null)
                        {
                            _logger.LogInformation("Received message body: {@body}",
                                receivedMessage.Body.ToString());
                        }
                        else
                        {
                            _logger.LogWarning("No message received from the queue: {@q_name}", _sbQueue);
                        }

                        _logger.LogTrace("Disposing client");
                        await _serviceBusClient.DisposeClientAsync();
                    }
                }
                else
                {
                    _logger.LogError("Impossible to create a ServiceBusClient");
                }

                _logger.LogDebug("Received the next FIFO message from the Queue");

                _logger.LogTrace("Method end");
            }
        }

        private async Task DeleteAllMessages()
        {
            using (_logger.BeginScope("DeleteAllMessages()"))
            {
                _logger.LogTrace("Method start");

                // Use Queue messaging receiver
                _logger.LogDebug("Delete all messages from the Queue");

                _logger.LogTrace("Got a IServiceBusClient instance");

                if (_sbNamespace != null && _serviceBusClient.CreateClient(_sbNamespace, _receiverClientId))
                {
                    _logger.LogTrace("ServiceBusClient created");

                    if (_sbQueue != null)
                    {
                        await _serviceBusClient.DeleteAllMessagesAsync(_sbQueue);

                        _logger.LogTrace("Disposing client");
                        await _serviceBusClient.DisposeClientAsync();
                    }
                }
                else
                {
                    _logger.LogError("Impossible to create a ServiceBusClient");
                }

                _logger.LogDebug("Deleted all messages from the Queue");

                _logger.LogTrace("Method end");
            }
        }
        #endregion
    }
}
