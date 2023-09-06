using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using WebApp1.Models;
using Azure.Storage.Blobs.Models;

namespace WebApp1.Pages
{
    public class DropContentModel : PageModel
    {
        private readonly ILogger<DropContentModel> _logger;
        private readonly IConfiguration _configuration;

        public DropContentModel(
            ILogger<DropContentModel> logger,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            var stAcctName = _configuration.GetValue<string>("DropStorageAccountName");

            var blobServiceClient = new BlobServiceClient(
                new Uri($"https://{stAcctName}.blob.core.windows.net"),
                new DefaultAzureCredential());

            // Creating a container
            string containerName = "zachry-pcr2-poc-" + Guid.NewGuid().ToString();

            // Create the container and return a container client object
            _ = await blobServiceClient.CreateBlobContainerAsync(containerName);

            ViewData["actionDisplay"] = $"Created container {containerName} in Storage account {stAcctName}";
        }
    }
}
