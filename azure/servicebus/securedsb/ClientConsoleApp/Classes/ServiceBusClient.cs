﻿// <copyright file="ServiceBusClient.cs" company="PlaceholderCompany">
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

using Azure.Identity;
using Azure.Messaging.ServiceBus;
using ClientConsoleApp.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClientConsoleApp.Classes
{
    internal class ServiceBusClient : IServiceBusClient
    {
        // Private members
        private readonly ILogger _logger;
        private Azure.Messaging.ServiceBus.ServiceBusClient? _sbClient;

        // Constructor
        public ServiceBusClient(ILogger<ServiceBusClient> logger)
        {
            _logger = logger;
        }

        // Interface implementation
        public bool CreateClient(string sbNamespace, string? clientId = null)
        {
            // Create a ServiceBusClient that will authenticate through Active Directory

            // Reference for the ServiceBusClient: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet
            // Reference for authentication with Azure.Identity: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet#authenticating-with-azureidentity

            // See Client lifetime recommendations for wider use out of this POC: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet#client-lifetime

            _logger.LogInformation("Creating a ServiceBusClient to the namespace: {@sb_ns}, with MI \"{@client_id}\"", sbNamespace, clientId);
            var fullyQualifiedNamespace = $"{sbNamespace}.{Constants.SbPublicSuffix}";

            // Enforce TLS 1.2 to connect to Service Bus
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            try
            {
                if (string.IsNullOrEmpty(clientId))
                {
                    // Code for system-assigned managed identity:
                    _sbClient = new Azure.Messaging.ServiceBus.ServiceBusClient(fullyQualifiedNamespace, new DefaultAzureCredential());
                }
                else
                {
                    // Code for user-assigned managed identity:
                    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                        { ManagedIdentityClientId = clientId });
                    _sbClient = new Azure.Messaging.ServiceBus.ServiceBusClient(fullyQualifiedNamespace, credential);
                }

                _logger.LogInformation($"ServiceBusClient created");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"ServiceBusClient creation failed");
                return false;
            }
        }

        public async Task DisposeClientAsync()
        {
            _logger.LogDebug("Disposing the ServiceBusClient");
            if (_sbClient != null)
            {
                await _sbClient.DisposeAsync();
            }
        }

        public async Task<bool> SendMessageAsync(string queue, string message)
        {
            // Ref: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet#send-and-receive-a-message

            // create the sender
            _logger.LogInformation("Creating a ServiceBusSender for the queue: {@q_name}", queue);

            if (_sbClient == null)
            {
                _logger.LogError("Cannot send a message when ServiceBusClient is not created");
                return false;
            }

            var sender = _sbClient.CreateSender(queue);

            // create a message that we can send. UTF-8 encoding is used when providing a string.
            _logger.LogInformation("Creating the message with content: {@msg}", message);
            var sbMessage = new ServiceBusMessage(message);

            // send the message
            try
            {
                _logger.LogInformation("Sending the message to the queue");

                await sender.SendMessageAsync(sbMessage);

                _logger.LogInformation("Message sent to the queue");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Sending Message failed");
                return false;
            }
        }

        public async Task<ServiceBusReceivedMessage?> ReceiveMessageAsync(string queue)
        {
            // Ref: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet#send-and-receive-a-message

            // create a receiver that we can use to receive the message
            if (_sbClient == null)
            {
                _logger.LogError("Cannot receive a message when ServiceBusClient is not created");
                return null;
            }

            _logger.LogInformation("Creating a ServiceBusReceiver for the queue: {@q_name}", queue);
            var receiver = _sbClient.CreateReceiver(queue);

            try
            {
                _logger.LogInformation("Receiving the current FIFO message from queue: {@queue}", queue);

                var receivedMessage = await receiver.ReceiveMessageAsync();

                if (true /*all is good with message*/)
                {
                    // Complete the message
                    await receiver.CompleteMessageAsync(receivedMessage);
                    _logger.LogInformation("Message Received and Completed in queue");
                }
                //else
                //{
                //    // Manage other possible status for message:
                //    // - Abandon: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet#abandon-a-message
                //    // - Defer: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet#defer-a-message
                //    // - Dead-letter: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet#dead-letter-a-message
                //}

                return receivedMessage;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Receiving and Completing the message failed");
                return await Task.FromException<ServiceBusReceivedMessage?>(e);
            }
        }

        public async Task<bool> DeleteAllMessagesAsync(string queue)
        {
            // Initialization
            var maxMsg = 50;
            var maxWait = new TimeSpan(0, 0, 15);
            var messagesToDelete = true;

            // create a receiver that we can use to receive the message
            if (_sbClient == null)
            {
                _logger.LogError("Cannot receive a message when ServiceBusClient is not created");
                return false;
            }

            _logger.LogInformation("Creating a ServiceBusReceiver for the queue: {@q_name}", queue);
            var receiver = _sbClient.CreateReceiver(queue);

            _logger.LogDebug("Retrieving messages by batches of {@max_msg}, with a {@max_wait} timeout", maxMsg, maxWait);

            while (messagesToDelete)
            {
                _logger.LogDebug("Retrieving messages");
                var receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: maxMsg, maxWaitTime: maxWait);

                _logger.LogDebug("Processing response");
                if (receivedMessages.Any())
                {
                    _logger.LogInformation("Received {@count} messages to delete", receivedMessages.Count);

                    foreach (var receivedMessage in receivedMessages)
                    {
                        // get the message body as a string
                        var body = receivedMessage.Body.ToString();
                        _logger.LogInformation("Deleting message: {@body}", body);

                        await receiver.CompleteMessageAsync(receivedMessage);
                    }
                    _logger.LogDebug("Deleted the retrieved messages");
                }
                else
                {
                    messagesToDelete = false;
                }
            }

            return true;
        }
    }
}
