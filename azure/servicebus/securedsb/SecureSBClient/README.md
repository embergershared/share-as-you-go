# SecureSBClient (Service Bus)

## Overview

This .NET 6.0 console app is designed to validate the settings and demonstrate the use of an Azure Service Bus Premium deployed in a secured manner. It leverages Azure security features from a the Webjob running the app to:

- Check it can resolve the Service Bus namespace:
	- For resolution to work: VNet, Private Endpoint, Privated DNS Zones, DNS zones linking and Private DNS records must be all configured correctly between the `WebJob App Service` and the `Service Bus`.
- Use a Managed Identity to authenticate to the Service Bus queue and send messages:
	- It means the used Managed Identity must be assigned the role "Azure Service Bus Data Sender" at the queue scope.
- Use a Managed Identity (can be another one) to authenticate to the Service Bus queue and receive messages:
	- It means the used Managed Identity must be assigned the role "Azure Service Bus Data Receiver" at the queue scope.

## Typical output

The App running as a `WebJob` will log like this:
![Logs output excerpt](https://github.com/embergershared/share-as-you-go/blob/main/azure/servicebus/securedsb/SecureSBClient/img/2022-10-21_190655.png)


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
| SENDER_CLIENT_ID | The Client ID of the Managed Identity authorized to SEND messages to the queue through RBAC | Can be any Client ID in the same Azure AD tenant |
| RECEIVER_CLIENT_ID | The Client ID of the Managed Identity authorized to RECEIVE messages to the queue through RBAC | Can be any Client ID in the same Azure AD tenant |

## Service Bus Namespace parameters

A Secured Service Bus will have these settings (Non-exhaustive list and always depdendant of the context):
- Pricing tier: `Premium`, to:
	- [Allow access to Azure Service Bus namespace via private endpoints](https://learn.microsoft.com/en-us/azure/service-bus-messaging/private-link-service)
	- [Allow access to Azure Service Bus namespace from specific virtual networks](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-service-endpoints)
	- [Use Customer-managed keys for data at rest encryption](https://learn.microsoft.com/en-us/azure/service-bus-messaging/configure-customer-managed-key)
- [Disable Local Authentication](https://learn.microsoft.com/en-us/azure/service-bus-messaging/disable-local-authentication)
- [Minimum TLS version set to 1.2](https://learn.microsoft.com/en-us/azure/service-bus-messaging/transport-layer-security-configure-minimum-version)
- Connectivity method: `Private access`
- Create `Private endpoint connections`

## Service Bus Queue parameters

- Access control (IAM) configured with the Role assignments created for the consumers' apps Managed Identities. Here's an example:
![RBAC for MI at queue scope](https://github.com/embergershared/share-as-you-go/blob/main/azure/servicebus/securedsb/SecureSBClient/img/2022-10-21_184714.png)

## Local development & debug

Cloud Explorer had been retired in Visual Studio 2022 ([Announcement])(https://learn.microsoft.com/en-us/visualstudio/azure/vs-azure-tools-resources-managing-with-cloud-explorer?view=vs-2022) and that doesn't help....'

The technique I used was to create a Basic Service Bus namespace and connect to it through my development workstation.
When the appropriate Azure Account is set in Visual Studio Tools > Options > Azure Service Authentication:

<p align="center">
  <img src="https://github.com/embergershared/share-as-you-go/blob/main/azure/servicebus/securedsb/SecureSBClient/img/2022-10-21_192515.png" alt="Sublime's custom image"/>
</p>

the `Azure.Identity` package, without parameters (meaning here with `SENDER_CLIENT_ID` & `RECEIVER_CLIENT_ID` left empty), will use the account described above to authenticate.

It allows to debug the client and the Service Bus objects operating in the app code.


# References

[Azure security baseline for Service Bus](https://learn.microsoft.com/en-us/security/benchmark/azure/baselines/service-bus-messaging-security-baseline)

[Authenticate a managed identity with Azure Active Directory to access Azure Service Bus resources](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-managed-service-identity)

[Authenticate and authorize an application with Azure Active Directory to access Azure Service Bus entities](https://learn.microsoft.com/en-us/azure/service-bus-messaging/authenticate-application)

[Azure Policy Regulatory Compliance controls for Azure Service Bus Messaging](https://learn.microsoft.com/en-us/azure/service-bus-messaging/security-controls-policy)

## Log output example

```
[10/21/2022 23:14:07 > 5ad2b7: SYS INFO] Status changed to Initializing
[10/21/2022 23:14:07 > 5ad2b7: SYS INFO] Job directory change detected: Job file 'run.cmd' timestamp differs between source and working directories.
[10/21/2022 23:14:08 > 5ad2b7: SYS INFO] Run script 'run.cmd' with script host - 'WindowsScriptHost'
[10/21/2022 23:14:08 > 5ad2b7: SYS INFO] Status changed to Running
[10/21/2022 23:14:08 > 5ad2b7: INFO] 
[10/21/2022 23:14:08 > 5ad2b7: INFO] C:\local\Temp\jobs\triggered\SecureSBClient\jmmy22ec.j2o>dotnet SecureSBClient.dll  
[10/21/2022 23:14:08 > 5ad2b7: INFO] [23:14:08.980 INF] (SecureSBClient.Program.) Starting application
[10/21/2022 23:14:08 > 5ad2b7: INFO] [23:14:09.000 DBG] (SecureSBClient.Program.) Getting values from configuration
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.011 DBG] (SecureSBClient.Program.) Launching DNS Resolution
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.020 INF] (SecureSBClient.Classes.DnsResolver.) Resolving: sbn-<ServiceBusName>-secure.servicebus.windows.net.
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.107 INF] (SecureSBClient.Classes.DnsResolver.) Results from DNS Server: 168.63.129.16:53
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.108 INF] (SecureSBClient.Classes.DnsResolver.) Record: sbn-<ServiceBusName>-secure.servicebus.windows.net. 1800 IN CNAME sbn-<ServiceBusName>-secure.privatelink.servicebus.windows.net.
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.108 INF] (SecureSBClient.Classes.DnsResolver.) Record: sbn-<ServiceBusName>-secure.privatelink.servicebus.windows.net. 1800 IN A 10.1.0.5
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.109 DBG] (SecureSBClient.Program.) DNS Resolution succeeded
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.114 DBG] (SecureSBClient.Program.) Sending a message to the Service Bus Queue
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.115 VRB] (SecureSBClient.Program.) Got a IServiceBusClient instance
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.121 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusClient to the namespace: sbn-<ServiceBusName>-secure, with MI "70c382c9-XXX"
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.166 INF] (SecureSBClient.Classes.ServiceBusClient.) ServiceBusClient created
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.166 VRB] (SecureSBClient.Program.) ServiceBusClient created
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.166 VRB] (SecureSBClient.Program.) Sending 10 messages to the queue
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.167 VRB] (SecureSBClient.Program.) Sending message #1 to the queue
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.171 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusSender for the queue: queue1
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.174 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating the message with content: Message #1 sent at 10/21/2022 11:14 PM
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.177 INF] (SecureSBClient.Classes.ServiceBusClient.) Sending the message to the queue
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.971 INF] (SecureSBClient.Classes.ServiceBusClient.) Message sent to the queue
[10/21/2022 23:14:09 > 5ad2b7: INFO] [23:14:09.971 INF] (SecureSBClient.Program.) Message sent
[10/21/2022 23:14:10 > 5ad2b7: INFO] [23:14:10.984 VRB] (SecureSBClient.Program.) Sending message #2 to the queue
[10/21/2022 23:14:10 > 5ad2b7: INFO] [23:14:10.984 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusSender for the queue: queue1
.../...
[10/21/2022 23:14:20 > 5ad2b7: INFO] [23:14:20.544 VRB] (SecureSBClient.Program.) 10 messages sent to the queue
[10/21/2022 23:14:20 > 5ad2b7: INFO] [23:14:20.545 VRB] (SecureSBClient.Program.) Disposing client
[10/21/2022 23:14:20 > 5ad2b7: INFO] [23:14:20.546 DBG] (SecureSBClient.Classes.ServiceBusClient.) Disposing the ServiceBusClient
[10/21/2022 23:14:20 > 5ad2b7: INFO] [23:14:20.563 DBG] (SecureSBClient.Program.) Sent a message to the Service Bus Queue
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.581 DBG] (SecureSBClient.Program.) Receiving the next FIFO message from the Queue
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.582 VRB] (SecureSBClient.Program.) Got a IServiceBusClient instance
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.582 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusClient to the namespace: sbn-<ServiceBusName>-secure, with MI "700d7684-YYY"
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.582 INF] (SecureSBClient.Classes.ServiceBusClient.) ServiceBusClient created
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.582 VRB] (SecureSBClient.Program.) ServiceBusClient created
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.582 VRB] (SecureSBClient.Program.) Receiving 1 message from the queue
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.584 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusReceiver for the queue: queue1
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.594 INF] (SecureSBClient.Classes.ServiceBusClient.) Receiving the current FIFO message from queue: queue1
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.949 INF] (SecureSBClient.Classes.ServiceBusClient.) Message Received and Completed in queue
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.951 INF] (SecureSBClient.Program.) Received message body: Message #1 sent at 10/21/2022 11:14 PM
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.951 VRB] (SecureSBClient.Program.) Disposing client
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.951 DBG] (SecureSBClient.Classes.ServiceBusClient.) Disposing the ServiceBusClient
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.952 DBG] (SecureSBClient.Program.) Received the next FIFO message from the Queue
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.956 DBG] (SecureSBClient.Program.) Delete all messages from the Queue
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.956 VRB] (SecureSBClient.Program.) Got a IServiceBusClient instance
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.956 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusClient to the namespace: sbn-<ServiceBusName>-secure, with MI "700d7684-ZZZ"
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.957 INF] (SecureSBClient.Classes.ServiceBusClient.) ServiceBusClient created
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.957 VRB] (SecureSBClient.Program.) ServiceBusClient created
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.961 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusReceiver for the queue: queue1
[10/21/2022 23:14:22 > 5ad2b7: INFO] [23:14:22.961 DBG] (SecureSBClient.Classes.ServiceBusClient.) Retrieving messages by batches of 50, with a 00:00:15 timeout
[10/21/2022 23:14:23 > 5ad2b7: INFO] [23:14:23.066 INF] (SecureSBClient.Classes.ServiceBusClient.) Received 9 messages to delete
[10/21/2022 23:14:23 > 5ad2b7: INFO] [23:14:23.067 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #2 sent at 10/21/2022 11:14 PM
[10/21/2022 23:14:23 > 5ad2b7: INFO] [23:14:23.086 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #3 sent at 10/21/2022 11:14 PM
[10/21/2022 23:14:23 > 5ad2b7: INFO] [23:14:23.117 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #4 sent at 10/21/2022 11:14 PM
[10/21/2022 23:14:23 > 5ad2b7: INFO] [23:14:23.148 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #5 sent at 10/21/2022 11:14 PM
[10/21/2022 23:14:23 > 5ad2b7: INFO] [23:14:23.290 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #6 sent at 10/21/2022 11:14 PM
[10/21/2022 23:14:23 > 5ad2b7: INFO] [23:14:23.320 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #7 sent at 10/21/2022 11:14 PM
[10/21/2022 23:14:23 > 5ad2b7: INFO] [23:14:23.350 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #8 sent at 10/21/2022 11:14 PM
[10/21/2022 23:14:23 > 5ad2b7: INFO] [23:14:23.381 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #9 sent at 10/21/2022 11:14 PM
[10/21/2022 23:14:23 > 5ad2b7: INFO] [23:14:23.413 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #10 sent at 10/21/2022 11:14 PM
[10/21/2022 23:14:23 > 5ad2b7: INFO] [23:14:23.445 DBG] (SecureSBClient.Classes.ServiceBusClient.) Deleted the retrieved messages
[10/21/2022 23:14:38 > 5ad2b7: INFO] [23:14:38.465 VRB] (SecureSBClient.Program.) Disposing client
[10/21/2022 23:14:38 > 5ad2b7: INFO] [23:14:38.465 DBG] (SecureSBClient.Classes.ServiceBusClient.) Disposing the ServiceBusClient
[10/21/2022 23:14:38 > 5ad2b7: INFO] [23:14:38.465 DBG] (SecureSBClient.Program.) Deleted all messages from the Queue
[10/21/2022 23:14:38 > 5ad2b7: INFO] [23:14:38.465 INF] (SecureSBClient.Program.) Ending application
[10/21/2022 23:14:38 > 5ad2b7: SYS INFO] Status changed to Success
```