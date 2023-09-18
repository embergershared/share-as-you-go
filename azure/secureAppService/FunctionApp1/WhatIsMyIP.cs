using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class WhatIsMyIp
    {
        [FunctionName("WhatIsMyIP")]
        public async Task Run([TimerTrigger("*/10 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            log.LogInformation("Querying the Public IP");

            log.LogInformation("   Using HttpClient");

            HttpClient httpClient = new()
            {
                BaseAddress = new Uri("http://icanhazip.com/"),
            };

            using var response = await httpClient.GetAsync("/");

            response.EnsureSuccessStatusCode();
                //.WriteRequestToConsole();
            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[Console] The Public IP seen is: {body}");
            log.LogInformation($"[Log] The Public IP seen is: {body}");

            //log.LogInformation("   Using WebClient");
            //var client = new WebClient();
            //var content = client.DownloadString("http://icanhazip.com/");

            //log.LogInformation($"The Public IP of this function app is: {content}");
        }
    }

    static class HttpResponseMessageExtensions
    {
        internal static void WriteRequestToConsole(this HttpResponseMessage response)
        {
            if (response is null)
            {
                return;
            }

            var request = response.RequestMessage;
            Console.Write($"{request?.Method} ");
            Console.Write($"{request?.RequestUri} ");
            Console.WriteLine($"HTTP/{request?.Version}");
        }
    }
}
