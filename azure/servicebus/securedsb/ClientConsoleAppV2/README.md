# SecureSBClient (Service Bus)

## Overview

This .NET 6.0 console app is designed to prove the concept of leveraging an Azure Service Bus Premium deployed in a secured manner, from an App Service, also deployed securely with VNet integration, using Azure RBAC to control queue access and permissions.

The `sender` and `receiver` objects in the App Service WebJob access 1 specific queue with the credentials from the `Managed identity(/ies)` assigned to the App Service.

The App does:

- Check it can resolve the Service Bus namespace:
  - For resolution to work: VNet, Private Endpoint, Private DNS Zones, DNS zones linking and Private DNS records must all be configured correctly between the `WebJob App Service` and the `Service Bus`.
- Use a Managed Identity to authenticate to the Service Bus queue and **send** messages:
  - It means the Managed Identity used must be assigned the role "Azure Service Bus Data Sender" at the queue scope.

- Use a Managed Identity (can be another one) to authenticate to the Service Bus queue and **receive** messages:
  - It means the used Managed Identity must be assigned the role "Azure Service Bus Data Receiver" at the queue scope.

## Typical output

The App running as a `WebJob` log will look like this:

<p align="center">
  <img src="https://github.com/embergershared/share-as-you-go/raw/main/azure/servicebus/securedsb/ClientConsoleAppV2/img/2022-10-21_190655.png" alt="Logs output excerpt" width="70%;">
</p>

## Main NuGet packages used

This app uses these NuGet packages:
| Packages | Version | Usage |
|------|-------|-------------|
| Azure.Identity | v1.8.0 | Provides authentication context for Azure |
| Azure.Messaging.ServiceBus | v7.11.1 | Allows to connect, send and receive messages to/from an Azure Service Bus |
| DnsClient | v1.7.0 | Provide DNS resolution and queries |
| Microsoft.Extensions.DependencyInjection.* | v7.0.0 + multiple Packages | Provides .NET DI |
| Microsoft.Extensions.Hosting.* | v7.0.0 + multiple Packages | Provides .NET Hosting |
| Microsoft.Extensions.Logging.* | v7.0.0 + multiple Packages | Provides .NET Logging |

## App deployment in Azure

This code runs as a WebJob in an App Service set with these characteristics:

- App Service Plan:
  - OS: Windows
  - SKU: P1v2

- App Service:
  - Publish: Code
  - Runtime stack: .NET 6 LTS
  - OS: Windows
  - Deployment: Disable Continuous deployment
  - Networking:
    - Enable network injection: ON
    - VNet integration (VNet has to be in the same region)
    - Inbound access / Enable private endpoints: OFF
    - Outbound access / Enable VNet integration: ON / Outbound subnet: set to a subnet
  - Set App Service Identity / System assigned to ON

- Publish profile:
  - pushes the console App as a Web Job in the App Service
  - Once run, the logs (dotnet console outputs leveraging SeriLog) can be seen in the WebJob Logs page

## Configuration

The 4 following Keys are expected in the App Service Configuration:
| Name | Value | Comments |
|------|-------|-------------|
| SERVICEBUS_NS_NAME | The Service Bus namespace name | The name without the full FQDN |
| QUEUE_NAME | Name of the queue to use in the Service Bus namespace | As we secure at the queue level, the queue name must be known by the App to connect to the queue |
| SENDER_CLIENT_ID | The Client ID of the Managed Identity authorized to SEND messages to the queue through RBAC | If null or empty, the system-managed identity will be used |
| RECEIVER_CLIENT_ID | The Client ID of the Managed Identity authorized to RECEIVE messages to the queue through RBAC | If null or empty, the system-managed identity will be used |

## Service Bus Namespace parameters

A Secured Service Bus will have these settings (Non-exhaustive list and always dependant of the context):

- Pricing tier `Premium`, to:
  - [Allow access to Azure Service Bus namespace via private endpoints](https://learn.microsoft.com/en-us/azure/service-bus-messaging/private-link-service)
  - [Allow access to Azure Service Bus namespace from specific virtual networks](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-service-endpoints)
  - [Use Customer-managed keys for data at rest encryption](https://learn.microsoft.com/en-us/azure/service-bus-messaging/configure-customer-managed-key)
- [Disable Local Authentication](https://learn.microsoft.com/en-us/azure/service-bus-messaging/disable-local-authentication)
- [Minimum TLS version set to 1.2](https://learn.microsoft.com/en-us/azure/service-bus-messaging/transport-layer-security-configure-minimum-version)
- Connectivity method: `Private access`
- Create `Private endpoint connections`

## Service Bus Queue parameters

Access control (IAM) configured with the Role assignments created for the consumers' apps Managed Identities. Here's an example:

<p align="center">
  <img src="https://github.com/embergershared/share-as-you-go/blob/main/azure/servicebus/securedsb/ClientConsoleAppV2/img/2022-10-21_184714.png" alt="SbQueueRBAC"/>
</p>

## Local development & debug

Cloud Explorer had been retired in Visual Studio 2022 ([Announcement](https://learn.microsoft.com/en-us/visualstudio/azure/vs-azure-tools-resources-managing-with-cloud-explorer?view=vs-2022)) and that doesn't help...

The technique I used was to create a Basic Service Bus namespace and connect to it through my development workstation.

When the appropriate Azure Account is set in Visual Studio Tools > Options > Azure Service Authentication:

<p align="center">
  <img src="https://github.com/embergershared/share-as-you-go/blob/main/azure/servicebus/securedsb/ClientConsoleAppV2/img/2022-10-21_192515.png" alt="VsToolsOptions"/>
</p>

the `Azure.Identity` package, without parameters (meaning here with `SENDER_CLIENT_ID` & `RECEIVER_CLIENT_ID` left empty), will use the account linked to your Visual Studio to authenticate.

It allows to debug the client and the Service Bus objects operating in the app code.

## References

[Azure security baseline for Service Bus](https://learn.microsoft.com/en-us/security/benchmark/azure/baselines/service-bus-messaging-security-baseline)

[Authenticate a managed identity with Azure Active Directory to access Azure Service Bus resources](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-managed-service-identity)

[Authenticate and authorize an application with Azure Active Directory to access Azure Service Bus entities](https://learn.microsoft.com/en-us/azure/service-bus-messaging/authenticate-application)

[Azure Policy Regulatory Compliance controls for Azure Service Bus Messaging](https://learn.microsoft.com/en-us/azure/service-bus-messaging/security-controls-policy)

## Log output example

```cmd
[11/10/2022 02:09:56 > 7d47fc: INFO] C:\local\Temp\jobs\triggered\ClientConsoleAppV2\fglwyceg.4ao>dotnet ClientConsoleAppV2.dll  
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.143 info: ClientConsoleAppV2.Program[0] Program started
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.157 trce: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() Method start
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.158 trce: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => GetVarValues() Method start
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.172 dbug: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => GetVarValues() TEST_PRIVATE_LINK: True
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.173 dbug: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => GetVarValues() SERVICEBUS_NS_NAME: sbn-emberger-secure
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.173 dbug: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => GetVarValues() QUEUE_NAME: urgent-queue
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.173 dbug: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => GetVarValues() SENDER_CLIENT_ID: 70c382c9-af8d-4aae-9167-e20d26d7f42a
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.174 dbug: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => GetVarValues() RECEIVER_CLIENT_ID: 700d7684-0ef2-423a-a564-fef32eae408e
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.174 trce: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => GetVarValues() Method end
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.175 trce: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => CheckDnsResolution() Method start
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.175 dbug: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => CheckDnsResolution() Launching DNS Resolution
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.181 info: ClientConsoleAppV2.Classes.DnsResolver[0] => ExecuteAsync() => CheckDnsResolution() Resolving: sbn-emberger-secure.servicebus.windows.net.
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.278 info: ClientConsoleAppV2.Classes.DnsResolver[0] => ExecuteAsync() => CheckDnsResolution() Results from DNS Server: 168.63.129.16:53
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.279 info: ClientConsoleAppV2.Classes.DnsResolver[0] => ExecuteAsync() => CheckDnsResolution() Record: sbn-emberger-secure.servicebus.windows.net. 1800 IN CNAME sbn-emberger-secure.privatelink.servicebus.windows.net.
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.279 info: ClientConsoleAppV2.Classes.DnsResolver[0] => ExecuteAsync() => CheckDnsResolution() Record: sbn-emberger-secure.privatelink.servicebus.windows.net. 60 IN A 10.1.0.4
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.279 info: ClientConsoleAppV2.Classes.DnsResolver[0] => ExecuteAsync() => CheckDnsResolution() Resolving: sbn-emberger-secure.privatelink.servicebus.windows.net.
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.285 info: ClientConsoleAppV2.Classes.DnsResolver[0] => ExecuteAsync() => CheckDnsResolution() Results from DNS Server: 168.63.129.16:53
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.285 info: ClientConsoleAppV2.Classes.DnsResolver[0] => ExecuteAsync() => CheckDnsResolution() Record: sbn-emberger-secure.privatelink.servicebus.windows.net. 60 IN A 10.1.0.4
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.285 dbug: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => CheckDnsResolution() DNS Resolution succeeded
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.285 trce: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => CheckDnsResolution() Method end
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.288 trce: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => SendMessages() Method start
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.289 trce: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => SendMessages() Got a IServiceBusClient instance
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.295 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Creating a ServiceBusClient to the namespace: sbn-emberger-secure, with MI "70c382c9-af8d-4aae-9167-e20d26d7f42a"
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.336 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() ServiceBusClient created
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.336 trce: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => SendMessages() ServiceBusClient created
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.336 trce: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => SendMessages() Sending 10 messages to the queue
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.336 trce: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => SendMessages() Sending message #1 to the queue
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.337 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Creating a ServiceBusSender for the queue: urgent-queue
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.340 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Creating the message with content: Message #1 sent at 11/10/2022 02:09 AM
[11/10/2022 02:09:57 > 7d47fc: INFO] 2022-11-10 02:09:57.341 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Sending the message to the queue
[11/10/2022 02:09:58 > 7d47fc: INFO] 2022-11-10 02:09:58.448 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Message sent to the queue
[11/10/2022 02:09:58 > 7d47fc: INFO] 2022-11-10 02:09:58.449 info: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => SendMessages() Message sent
[11/10/2022 02:09:59 > 7d47fc: INFO] 2022-11-10 02:09:59.454 trce: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => SendMessages() Sending message #2 to the queue
[11/10/2022 02:09:59 > 7d47fc: INFO] 2022-11-10 02:09:59.455 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Creating a ServiceBusSender for the queue: urgent-queue
[11/10/2022 02:09:59 > 7d47fc: INFO] 2022-11-10 02:09:59.455 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Creating the message with content: Message #2 sent at 11/10/2022 02:09 AM
[11/10/2022 02:09:59 > 7d47fc: INFO] 2022-11-10 02:09:59.455 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Sending the message to the queue
[11/10/2022 02:09:59 > 7d47fc: INFO] 2022-11-10 02:09:59.510 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Message sent to the queue
[11/10/2022 02:09:59 > 7d47fc: INFO] 2022-11-10 02:09:59.510 info: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => SendMessages() Message sent
[11/10/2022 02:10:00 > 7d47fc: INFO] 2022-11-10 02:10:00.518 trce: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => SendMessages() Sending message #3 to the queue
[11/10/2022 02:10:00 > 7d47fc: INFO] 2022-11-10 02:10:00.519 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Creating a ServiceBusSender for the queue: urgent-queue
[11/10/2022 02:10:00 > 7d47fc: INFO] 2022-11-10 02:10:00.519 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Creating the message with content: Message #3 sent at 11/10/2022 02:10 AM
[11/10/2022 02:10:00 > 7d47fc: INFO] 2022-11-10 02:10:00.519 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Sending the message to the queue
[11/10/2022 02:10:00 > 7d47fc: INFO] 2022-11-10 02:10:00.569 info: ClientConsoleAppV2.Classes.ServiceBusClient[0] => ExecuteAsync() => SendMessages() Message sent to the queue
[11/10/2022 02:10:00 > 7d47fc: INFO] 2022-11-10 02:10:00.570 info: ClientConsoleAppV2.Classes.ConsoleExecute[0] => ExecuteAsync() => SendMessages() Message sent```
