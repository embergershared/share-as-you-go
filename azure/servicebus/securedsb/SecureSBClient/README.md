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

```
[10/21/2022 22:01:07 > 5ad2b7: SYS INFO] Status changed to Initializing
[10/21/2022 22:01:07 > 5ad2b7: SYS INFO] Job directory change detected: Job file 'run.cmd' timestamp differs between source and working directories.
[10/21/2022 22:01:08 > 5ad2b7: SYS INFO] Run script 'run.cmd' with script host - 'WindowsScriptHost'
[10/21/2022 22:01:08 > 5ad2b7: SYS INFO] Status changed to Running
[10/21/2022 22:01:08 > 5ad2b7: INFO] 
[10/21/2022 22:01:08 > 5ad2b7: INFO] C:\local\Temp\jobs\triggered\SecureSBClient\kjw50odu.o5g>dotnet SecureSBClient.dll  
[10/21/2022 22:01:08 > 5ad2b7: INFO] [22:01:08.955 INF] (SecureSBClient.Program.) Starting application
[10/21/2022 22:01:08 > 5ad2b7: INFO] [22:01:08.973 DBG] (SecureSBClient.Program.) Getting values from configuration
[10/21/2022 22:01:08 > 5ad2b7: INFO] [22:01:08.984 DBG] (SecureSBClient.Program.) Launching DNS Resolution
[10/21/2022 22:01:08 > 5ad2b7: INFO] [22:01:08.993 INF] (SecureSBClient.Classes.DnsResolver.) Resolving: sbn-<ServiceBusName>-secure.servicebus.windows.net.
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.081 INF] (SecureSBClient.Classes.DnsResolver.) Results from DNS Server: 168.63.129.16:53
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.082 INF] (SecureSBClient.Classes.DnsResolver.) Record: sbn-<ServiceBusName>-secure.servicebus.windows.net. 1800 IN CNAME sbn-<ServiceBusName>-secure.privatelink.servicebus.windows.net.
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.082 INF] (SecureSBClient.Classes.DnsResolver.) Record: sbn-<ServiceBusName>-secure.privatelink.servicebus.windows.net. 1800 IN A 10.1.0.5
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.083 DBG] (SecureSBClient.Program.) DNS Resolution succeeded
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.088 DBG] (SecureSBClient.Program.) Sending a message to the Service Bus Queue
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.088 VRB] (SecureSBClient.Program.) Got a IServiceBusClient instance
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.096 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusClient to the namespace: sbn-<ServiceBusName>-secure, with MI "70c382c9-"
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.149 INF] (SecureSBClient.Classes.ServiceBusClient.) ServiceBusClient created
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.149 VRB] (SecureSBClient.Program.) ServiceBusClient created
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.149 VRB] (SecureSBClient.Program.) Sending 10 messages to the queue
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.150 VRB] (SecureSBClient.Program.) Sending message #1 to the queue
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.154 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusSender for the queue: queue1
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.158 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating the message with content: Message #1 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:09 > 5ad2b7: INFO] [22:01:09.160 INF] (SecureSBClient.Classes.ServiceBusClient.) Sending the message to the queue
[10/21/2022 22:01:10 > 5ad2b7: INFO] [22:01:10.552 INF] (SecureSBClient.Classes.ServiceBusClient.) Message sent to the queue
[10/21/2022 22:01:10 > 5ad2b7: INFO] [22:01:10.553 INF] (SecureSBClient.Program.) Message sent
[10/21/2022 22:01:11 > 5ad2b7: INFO] [22:01:11.566 VRB] (SecureSBClient.Program.) Sending message #2 to the queue
[10/21/2022 22:01:11 > 5ad2b7: INFO] [22:01:11.567 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusSender for the queue: queue1
[10/21/2022 22:01:11 > 5ad2b7: INFO] [22:01:11.567 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating the message with content: Message #2 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:11 > 5ad2b7: INFO] [22:01:11.567 INF] (SecureSBClient.Classes.ServiceBusClient.) Sending the message to the queue
[10/21/2022 22:01:11 > 5ad2b7: INFO] [22:01:11.619 INF] (SecureSBClient.Classes.ServiceBusClient.) Message sent to the queue
[10/21/2022 22:01:11 > 5ad2b7: INFO] [22:01:11.619 INF] (SecureSBClient.Program.) Message sent
[10/21/2022 22:01:12 > 5ad2b7: INFO] [22:01:12.620 VRB] (SecureSBClient.Program.) Sending message #3 to the queue
[10/21/2022 22:01:12 > 5ad2b7: INFO] [22:01:12.621 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusSender for the queue: queue1
[10/21/2022 22:01:12 > 5ad2b7: INFO] [22:01:12.621 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating the message with content: Message #3 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:12 > 5ad2b7: INFO] [22:01:12.621 INF] (SecureSBClient.Classes.ServiceBusClient.) Sending the message to the queue
[10/21/2022 22:01:12 > 5ad2b7: INFO] [22:01:12.671 INF] (SecureSBClient.Classes.ServiceBusClient.) Message sent to the queue
[10/21/2022 22:01:12 > 5ad2b7: INFO] [22:01:12.672 INF] (SecureSBClient.Program.) Message sent
[10/21/2022 22:01:13 > 5ad2b7: INFO] [22:01:13.684 VRB] (SecureSBClient.Program.) Sending message #4 to the queue
[10/21/2022 22:01:13 > 5ad2b7: INFO] [22:01:13.684 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusSender for the queue: queue1
[10/21/2022 22:01:13 > 5ad2b7: INFO] [22:01:13.684 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating the message with content: Message #4 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:13 > 5ad2b7: INFO] [22:01:13.684 INF] (SecureSBClient.Classes.ServiceBusClient.) Sending the message to the queue
[10/21/2022 22:01:13 > 5ad2b7: INFO] [22:01:13.737 INF] (SecureSBClient.Classes.ServiceBusClient.) Message sent to the queue
[10/21/2022 22:01:13 > 5ad2b7: INFO] [22:01:13.738 INF] (SecureSBClient.Program.) Message sent
[10/21/2022 22:01:14 > 5ad2b7: INFO] [22:01:14.742 VRB] (SecureSBClient.Program.) Sending message #5 to the queue
[10/21/2022 22:01:14 > 5ad2b7: INFO] [22:01:14.743 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusSender for the queue: queue1
[10/21/2022 22:01:14 > 5ad2b7: INFO] [22:01:14.743 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating the message with content: Message #5 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:14 > 5ad2b7: INFO] [22:01:14.743 INF] (SecureSBClient.Classes.ServiceBusClient.) Sending the message to the queue
[10/21/2022 22:01:14 > 5ad2b7: INFO] [22:01:14.794 INF] (SecureSBClient.Classes.ServiceBusClient.) Message sent to the queue
[10/21/2022 22:01:14 > 5ad2b7: INFO] [22:01:14.794 INF] (SecureSBClient.Program.) Message sent
[10/21/2022 22:01:15 > 5ad2b7: INFO] [22:01:15.801 VRB] (SecureSBClient.Program.) Sending message #6 to the queue
[10/21/2022 22:01:15 > 5ad2b7: INFO] [22:01:15.802 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusSender for the queue: queue1
[10/21/2022 22:01:15 > 5ad2b7: INFO] [22:01:15.802 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating the message with content: Message #6 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:15 > 5ad2b7: INFO] [22:01:15.802 INF] (SecureSBClient.Classes.ServiceBusClient.) Sending the message to the queue
[10/21/2022 22:01:15 > 5ad2b7: INFO] [22:01:15.852 INF] (SecureSBClient.Classes.ServiceBusClient.) Message sent to the queue
[10/21/2022 22:01:15 > 5ad2b7: INFO] [22:01:15.852 INF] (SecureSBClient.Program.) Message sent
[10/21/2022 22:01:16 > 5ad2b7: INFO] [22:01:16.856 VRB] (SecureSBClient.Program.) Sending message #7 to the queue
[10/21/2022 22:01:16 > 5ad2b7: INFO] [22:01:16.856 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusSender for the queue: queue1
[10/21/2022 22:01:16 > 5ad2b7: INFO] [22:01:16.856 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating the message with content: Message #7 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:16 > 5ad2b7: INFO] [22:01:16.856 INF] (SecureSBClient.Classes.ServiceBusClient.) Sending the message to the queue
[10/21/2022 22:01:16 > 5ad2b7: INFO] [22:01:16.924 INF] (SecureSBClient.Classes.ServiceBusClient.) Message sent to the queue
[10/21/2022 22:01:16 > 5ad2b7: INFO] [22:01:16.925 INF] (SecureSBClient.Program.) Message sent
[10/21/2022 22:01:17 > 5ad2b7: INFO] [22:01:17.940 VRB] (SecureSBClient.Program.) Sending message #8 to the queue
[10/21/2022 22:01:17 > 5ad2b7: INFO] [22:01:17.940 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusSender for the queue: queue1
[10/21/2022 22:01:17 > 5ad2b7: INFO] [22:01:17.940 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating the message with content: Message #8 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:17 > 5ad2b7: INFO] [22:01:17.940 INF] (SecureSBClient.Classes.ServiceBusClient.) Sending the message to the queue
[10/21/2022 22:01:17 > 5ad2b7: INFO] [22:01:17.996 INF] (SecureSBClient.Classes.ServiceBusClient.) Message sent to the queue
[10/21/2022 22:01:17 > 5ad2b7: INFO] [22:01:17.997 INF] (SecureSBClient.Program.) Message sent
[10/21/2022 22:01:19 > 5ad2b7: INFO] [22:01:19.007 VRB] (SecureSBClient.Program.) Sending message #9 to the queue
[10/21/2022 22:01:19 > 5ad2b7: INFO] [22:01:19.008 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusSender for the queue: queue1
[10/21/2022 22:01:19 > 5ad2b7: INFO] [22:01:19.008 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating the message with content: Message #9 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:19 > 5ad2b7: INFO] [22:01:19.008 INF] (SecureSBClient.Classes.ServiceBusClient.) Sending the message to the queue
[10/21/2022 22:01:19 > 5ad2b7: INFO] [22:01:19.082 INF] (SecureSBClient.Classes.ServiceBusClient.) Message sent to the queue
[10/21/2022 22:01:19 > 5ad2b7: INFO] [22:01:19.082 INF] (SecureSBClient.Program.) Message sent
[10/21/2022 22:01:20 > 5ad2b7: INFO] [22:01:20.085 VRB] (SecureSBClient.Program.) Sending message #10 to the queue
[10/21/2022 22:01:20 > 5ad2b7: INFO] [22:01:20.085 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusSender for the queue: queue1
[10/21/2022 22:01:20 > 5ad2b7: INFO] [22:01:20.085 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating the message with content: Message #10 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:20 > 5ad2b7: INFO] [22:01:20.085 INF] (SecureSBClient.Classes.ServiceBusClient.) Sending the message to the queue
[10/21/2022 22:01:20 > 5ad2b7: INFO] [22:01:20.136 INF] (SecureSBClient.Classes.ServiceBusClient.) Message sent to the queue
[10/21/2022 22:01:20 > 5ad2b7: INFO] [22:01:20.137 INF] (SecureSBClient.Program.) Message sent
[10/21/2022 22:01:21 > 5ad2b7: INFO] [22:01:21.148 VRB] (SecureSBClient.Program.) 10 messages sent to the queue
[10/21/2022 22:01:21 > 5ad2b7: INFO] [22:01:21.148 VRB] (SecureSBClient.Program.) Disposing client
[10/21/2022 22:01:21 > 5ad2b7: INFO] [22:01:21.149 DBG] (SecureSBClient.Classes.ServiceBusClient.) Disposing the ServiceBusClient
[10/21/2022 22:01:21 > 5ad2b7: INFO] [22:01:21.166 DBG] (SecureSBClient.Program.) Sent a message to the Service Bus Queue
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.180 DBG] (SecureSBClient.Program.) Receiving the next FIFO message from the Queue
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.180 VRB] (SecureSBClient.Program.) Got a IServiceBusClient instance
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.181 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusClient to the namespace: sbn-<ServiceBusName>-secure, with MI "700d7684-"
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.181 INF] (SecureSBClient.Classes.ServiceBusClient.) ServiceBusClient created
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.181 VRB] (SecureSBClient.Program.) ServiceBusClient created
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.181 VRB] (SecureSBClient.Program.) Receiving 1 message from the queue
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.183 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusReceiver for the queue: queue1
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.194 INF] (SecureSBClient.Classes.ServiceBusClient.) Receiving the current FIFO message from queue: queue1
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.597 INF] (SecureSBClient.Classes.ServiceBusClient.) Message Received and Completed in queue
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.599 INF] (SecureSBClient.Program.) Received message body: Message #4 sent at 10/21/2022 07:02 PM
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.599 VRB] (SecureSBClient.Program.) Disposing client
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.599 DBG] (SecureSBClient.Classes.ServiceBusClient.) Disposing the ServiceBusClient
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.599 DBG] (SecureSBClient.Program.) Received the next FIFO message from the Queue
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.604 DBG] (SecureSBClient.Program.) Delete all messages from the Queue
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.604 VRB] (SecureSBClient.Program.) Got a IServiceBusClient instance
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.604 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusClient to the namespace: sbn-<ServiceBusName>-secure, with MI "700d7684-"
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.604 INF] (SecureSBClient.Classes.ServiceBusClient.) ServiceBusClient created
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.604 VRB] (SecureSBClient.Program.) ServiceBusClient created
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.608 INF] (SecureSBClient.Classes.ServiceBusClient.) Creating a ServiceBusReceiver for the queue: queue1
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.608 DBG] (SecureSBClient.Classes.ServiceBusClient.) Retrieving messages by batches of 50, with a 00:00:15 timeout
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.735 INF] (SecureSBClient.Classes.ServiceBusClient.) Received 36 messages to delete
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.735 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #5 sent at 10/21/2022 07:02 PM
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.767 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #6 sent at 10/21/2022 07:02 PM
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.796 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #7 sent at 10/21/2022 07:02 PM
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.828 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #8 sent at 10/21/2022 07:02 PM
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.860 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #9 sent at 10/21/2022 07:02 PM
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.890 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #10 sent at 10/21/2022 07:02 PM
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.920 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #1 sent at 10/21/2022 07:16 PM
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.952 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #2 sent at 10/21/2022 07:16 PM
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:23.983 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #3 sent at 10/21/2022 07:16 PM
[10/21/2022 22:01:23 > 5ad2b7: INFO] [22:01:24.013 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #4 sent at 10/21/2022 07:16 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.045 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #5 sent at 10/21/2022 07:16 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.078 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #6 sent at 10/21/2022 07:16 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.109 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #7 sent at 10/21/2022 07:16 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.145 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #8 sent at 10/21/2022 07:16 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.171 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #9 sent at 10/21/2022 07:16 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.202 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #10 sent at 10/21/2022 07:16 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.233 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #1 sent at 10/21/2022 07:27 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.265 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #2 sent at 10/21/2022 07:27 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.295 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #3 sent at 10/21/2022 07:27 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.326 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #4 sent at 10/21/2022 07:27 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.358 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #5 sent at 10/21/2022 07:27 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.390 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #6 sent at 10/21/2022 07:27 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.431 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #7 sent at 10/21/2022 07:27 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.451 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #8 sent at 10/21/2022 07:27 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.486 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #9 sent at 10/21/2022 07:27 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.610 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #10 sent at 10/21/2022 07:27 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.640 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #1 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.765 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #2 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.796 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #3 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.828 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #4 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.912 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #5 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.937 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #6 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:24.968 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #7 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:24 > 5ad2b7: INFO] [22:01:25.000 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #8 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:25 > 5ad2b7: INFO] [22:01:25.030 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #9 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:25 > 5ad2b7: INFO] [22:01:25.065 INF] (SecureSBClient.Classes.ServiceBusClient.) Deleting message: Message #10 sent at 10/21/2022 10:01 PM
[10/21/2022 22:01:25 > 5ad2b7: INFO] [22:01:25.095 DBG] (SecureSBClient.Classes.ServiceBusClient.) Deleted the retrieved messages
[10/21/2022 22:01:40 > 5ad2b7: INFO] [22:01:40.104 VRB] (SecureSBClient.Program.) Disposing client
[10/21/2022 22:01:40 > 5ad2b7: INFO] [22:01:40.105 DBG] (SecureSBClient.Classes.ServiceBusClient.) Disposing the ServiceBusClient
[10/21/2022 22:01:40 > 5ad2b7: INFO] [22:01:40.105 DBG] (SecureSBClient.Program.) Deleted all messages from the Queue
[10/21/2022 22:01:40 > 5ad2b7: INFO] [22:01:40.105 INF] (SecureSBClient.Program.) Ending application
[10/21/2022 22:01:40 > 5ad2b7: SYS INFO] Status changed to Success
```


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


# References

[Azure security baseline for Service Bus](https://learn.microsoft.com/en-us/security/benchmark/azure/baselines/service-bus-messaging-security-baseline)

[Authenticate a managed identity with Azure Active Directory to access Azure Service Bus resources](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-managed-service-identity)

[Authenticate and authorize an application with Azure Active Directory to access Azure Service Bus entities](https://learn.microsoft.com/en-us/azure/service-bus-messaging/authenticate-application)

[Azure Policy Regulatory Compliance controls for Azure Service Bus Messaging](https://learn.microsoft.com/en-us/azure/service-bus-messaging/security-controls-policy)