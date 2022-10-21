using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using SecureSBClient.Interfaces;

namespace SecureSBClient.Classes
{
    internal class QueueMessageSender : IQueueMessageSender
    {

        private readonly ILogger _logger;
        private ServiceBusClient? _sbClient;

        public QueueMessageSender(ILogger<QueueMessageSender> logger)
        {
            _logger = logger;
        }

        public bool CreateClient(string sbNamespace, string? clientId = null)
        {
            // Create a ServiceBusClient that will authenticate through Active Directory

            // Reference for the ServiceBusClient: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet
            // Reference for authentication with Azure.Identity: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme?view=azure-dotnet#authenticating-with-azureidentity

            _logger.LogInformation("Creating a ServiceBusClient to the namespace: {@sb_ns}, with MI \"{@client_id}\"", sbNamespace, clientId);
            var fullyQualifiedNamespace = $"{sbNamespace}.{Constants.sbPublicSuffix}";

            try
            {
                if (string.IsNullOrEmpty(clientId))
                {
                    // Code for system-assigned managed identity:
                    _sbClient = new ServiceBusClient(fullyQualifiedNamespace, new DefaultAzureCredential());
                }
                else
                {
                    // Code for user-assigned managed identity:
                    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                        { ManagedIdentityClientId = clientId });
                    _sbClient = new ServiceBusClient(fullyQualifiedNamespace, credential);
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

        public async Task<bool> SendMessageAsync(string queue, string message)
        {
            // create the sender
            _logger.LogInformation("Creating a ServiceBusSender to the queue: {@q_name}", queue);
            if (_sbClient != null)
            {
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
            else
            {
                _logger.LogError("Cannot send a message when ServiceBusClient is not created");
                return false;
            }
        }
    }
}
