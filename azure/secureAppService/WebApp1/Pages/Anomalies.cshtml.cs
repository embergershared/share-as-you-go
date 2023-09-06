using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.AnomalyDetector;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs.Models;

namespace WebApp1.Pages
{
    public class AnomaliesModel : PageModel
    {
        private readonly ILogger<AnomaliesModel> _logger;
        private readonly IConfiguration _configuration;

        public AnomaliesModel(
            ILogger<AnomaliesModel> logger,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            var endpoint = _configuration.GetValue<string>("adEndpoint");
            var apiKey = _configuration.GetValue<string>("adKey");

            var stAcctName = _configuration.GetValue<string>("ArchiveStorageAccountName");
            var contName = _configuration.GetValue<string>("ArchiveContainerName");
            var blobName = _configuration.GetValue<string>("ArchiveBlobName");

            //create AD client
            var endpointUri = new Uri(endpoint);
            var credential = new AzureKeyCredential(apiKey);
            AnomalyDetectorClient client = new AnomalyDetectorClient(endpointUri, credential);

            // create Storage account client
            var blobServiceClient = new BlobServiceClient(
                new Uri($"https://{stAcctName}.blob.core.windows.net"),
                new DefaultAzureCredential()
            );

            // Access container
            var blobContainerClient = blobServiceClient.GetBlobContainerClient("anomaly-data");
            // Access blob
            var blobClient = blobContainerClient.GetBlobClient("request-data.csv");

            // Download blob
            var content = DownloadBlobToStringAsync(blobClient).Result;

            // Process data input
            var data = content.Split("\r\n");
            var list2 = data
                .Select(line => new TimeSeriesPoint(float.Parse(line.Split(",")[1])) { Timestamp = DateTime.Parse(line.Split(",")[0]) })
                .ToList();

            //create request
            UnivariateDetectionOptions request = new UnivariateDetectionOptions(list2)
            {
                Granularity = TimeGranularity.Daily
            };

            UnivariateEntireDetectionResult result = client.DetectUnivariateEntireSeries(request);

            var output = string.Empty;
            bool hasAnomaly = false;
            for (int i = 0; i < request.Series.Count; ++i)
            {
                if (result.IsAnomaly[i])
                {
                    output += $"Anomaly detected at line: {i + 1}.\r\n";
                    hasAnomaly = true;
                }
            }
            if (!hasAnomaly)
            {
                output = "No anomalies detected in the series.";
            }

            ViewData["result"] = output;
        }

        public static async Task<string> DownloadBlobToStringAsync(BlobClient blobClient)
        {
            BlobDownloadResult downloadResult = await blobClient.DownloadContentAsync();
            return downloadResult.Content.ToString();
        }

    }
}
