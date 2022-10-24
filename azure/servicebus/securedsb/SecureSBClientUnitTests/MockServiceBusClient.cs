using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;


namespace ClientUnitTests
{
    public internal class MockServiceBusClient : ServiceBusClient
    {
        public override ServiceBusSender CreateSender(string queueOrTopicName, ServiceBusSenderOptions options)
        {
            return base.CreateSender(queueOrTopicName, options);
        }

        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }
    }
}
