using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace WebApp1.Pages
{
    public class ArchiveCModel : PageModel
    {

        private readonly ILogger<ArchiveCModel> _logger;
        private readonly IConfiguration _configuration;

        public ArchiveCModel(
            ILogger<ArchiveCModel> logger,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            var stAcctName = _configuration.GetValue<string>("ArchiveStorageAccountName");

            var blobServiceClient = new BlobServiceClient(
                new Uri($"https://{stAcctName}.blob.core.windows.net"),
                new DefaultAzureCredential());

            // Creating a container
            var containerName = "zachry-pcr2-poc-" + Guid.NewGuid().ToString();

            // Create the container and return a container client object
            _ = await blobServiceClient.CreateBlobContainerAsync(containerName);

            ViewData["actionDisplay"] = $"Created container {containerName} in Storage account {stAcctName}";
        }
    }
}
