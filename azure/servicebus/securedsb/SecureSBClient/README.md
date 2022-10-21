# SecureSBClient (Service Bus)

## Overview


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