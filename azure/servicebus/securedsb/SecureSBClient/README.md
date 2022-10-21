# SecureSBClient (Service Bus)

## Overview

<p>This .NET 6.0 console app is designed to validate the settings and demonstrate the use of an Azure Service Bus Premium deployed in a secured manner.<br>
It leverages the Private deployment in Azure to:</p>

- Check it can resolve the service bus namespace:
	- For resolution to work: VNet, Private Endpoint, Privated DNS Zones, DNS zones linking and Private DNS records must be all configured correctly between the `WebJob App Service` and the `Service Bus`.
- Use a Managed Identity to authenticate to the Service Bus queue and send messages:
	- It means the used Managed Identity must be assigned the role "Azure Service Bus Data Sender" at the queue scope.
- Use a Managed Identity (can be another one) to authenticate to the Service Bus queue and receive messages:
	- It means the used Managed Identity must be assigned the role "Azure Service Bus Data Receiver" at the queue scope.

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
| Name | Value | Description |
|------|-------|-------------|
| QUEUE_NAME | Name of the queue to use in the Service Bus namespace | As we secure at the queue level, the queue name must be known by the App to connect to the queue |
| SERVICEBUS_NS_NAME | The Service Bus namespace name | The name without the full FQDN |
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
![RBAC for MI at queue scope](/img/2022-10-21_184714.png)


# References

[Azure security baseline for Service Bus](https://learn.microsoft.com/en-us/security/benchmark/azure/baselines/service-bus-messaging-security-baseline)

[Authenticate a managed identity with Azure Active Directory to access Azure Service Bus resources](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-managed-service-identity)

[Authenticate and authorize an application with Azure Active Directory to access Azure Service Bus entities](https://learn.microsoft.com/en-us/azure/service-bus-messaging/authenticate-application)

[Azure Policy Regulatory Compliance controls for Azure Service Bus Messaging](https://learn.microsoft.com/en-us/azure/service-bus-messaging/security-controls-policy)