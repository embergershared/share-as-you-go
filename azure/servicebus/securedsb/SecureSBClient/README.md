# SecureSBClient (Service Bus)

## Overview

This .NET Core 5 console app is designed to validate the settings and behavior of an Azure Service Bus deployed in a secured manner.
It uses the Private deployment in Azure to:
- Check it can resolve the service bus namespace. For resolution to work, Private Endpoint, Privated DNS Zones, DNS zones linking and Private DNS records must be all configured.
- Use a Managed Identity to authenticate to the Service Bus queue and send messages. It means this MI must be have the role "Azure Service Bus Data Sender" assignment created to it at the queue scope.
- Use a Managed Identity (can be another one) to authenticate to the Service Bus queue and receive messages. It means this MI must be have the role "Azure Service Bus Data Receiver" assignment created to it at the queue scope.

## Client deployment in Azure

This code runs as a WebJobs in an App Service with these characteristics:
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
		- VNet integration. VNet has to be in the same region
		- Inbound access / Enable private endpoints: OFF
		- Outbound access / Enable VNet integration: ON / Outbound subnet: set to a subnet
	- Set App Service Identity / System assigned to ON

- Publish profile:
	- pushes the console App as a Web Job in the App Service
	- Once run, the logs (dotnet console outputs) can be seen here: https://securesbclient.scm.azurewebsites.net/azurejobs/#/jobs/triggered/SecureSBClient


## Service Bus queue parameter