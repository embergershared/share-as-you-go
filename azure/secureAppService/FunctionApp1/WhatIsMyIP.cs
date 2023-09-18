using System;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class WhatIsMyIP
    {
        [FunctionName("WhatIsMyIP")]
        public void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            log.LogInformation("Querying the Public IP");
            var client = new WebClient();
            var content = client.DownloadString("http://icanhazip.com/");

            log.LogInformation($"The Public IP of this function app is: {content}");
        }
    }
}
