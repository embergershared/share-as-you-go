using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure;

namespace FunctionApp1
{
    public class InputFilesProcessor
    {
        [FunctionName("InputFilesProcessor")]
        public async Task Run([TimerTrigger("*/15 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            // Inspiration: https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=visual-studio%2Cmanaged-identity%2Croles-azure-portal%2Csign-in-azure-cli%2Cidentity-visual-studio

            log.LogInformation($"C# Timer triggered function InputFilesProcessor STARTED at: {DateTime.Now}");

            // Preparing outputs
            var outputContent = string.Empty;

            
            // Grabbing Drop Storage Account & Container settings
            var dropStName = Environment.GetEnvironmentVariable("DropStorageAccountName") ??
                             throw new InvalidOperationException(
                                 "Environment Variable 'DropStorageAccountName' not found.");
            var dropContainerName = Environment.GetEnvironmentVariable("DropContainerName") ??
                                    throw new InvalidOperationException(
                                        "Environment Variable 'DropContainerName' not found.");


            // List and extract blobItems found
            var dropBlobContainerClient = GetBlobContainerClient(log, dropStName, dropContainerName);
            var dropBlobItems = new List<BlobItem>();

            log.LogInformation("Listing blobs found in {0}/ container", dropContainerName);
            await foreach (var blobItem in GetBlobItemsAsync(dropBlobContainerClient))
            {
                dropBlobItems.Add(blobItem);
                LogFoundBlobItem(log, blobItem);
            }


            // Remove folders from the drop Blobs' List
            _ = dropBlobItems.RemoveAll(x => x.Properties.ContentLength == 0);


            // Process each blobItem
            foreach (var dropBlobItem in dropBlobItems)
            {
                // Initialize an empty MemoryStream
                var memStream = new MemoryStream();
                memStream.Flush();
                memStream.Position = 0;

                // Download BlobItem content as Stream
                var blobItemClient = GetBlobClient(dropBlobContainerClient, dropBlobItem);
                log.LogInformation($"Loading {dropBlobItem.Name} content");

                // Not knowing the files formats, we use a MemoryStream.
                // If files are made of text (not bytes), things are easier with ToString()
                try
                {
                    // Note: Use DownloadContentAsync() for blobs that fit in memory, else use OpenReadAsync() and store locally.
                    var response = await blobItemClient.DownloadContentAsync();

                    if (response.GetRawResponse().ReasonPhrase == "OK")
                    {
                        await response.Value.Content.ToStream().CopyToAsync(memStream);

                        await LogBlobItemContentAsync(log, dropBlobItem, memStream);
                    }
                }
                catch (Exception ex) { log.LogError($"An Exception occurred while DownloadingContentAsync() of BlobItem {dropBlobItem.Name}: {ex}"); }

                outputContent += await ProcessDropBlobItemAsync(log, dropBlobItem, memStream);
            }

            // Create the Archive file
            var newArchiveName = $"{DateTime.Now:s}-drop-processing-output.txt";
            log.LogInformation($"Generating the new blob archive name: {newArchiveName}");


            // Get Archive Storage Account settings
            var archiveStName = Environment.GetEnvironmentVariable("ArchiveStorageAccountName") ??
                                    throw new InvalidOperationException(
                                        "Environment Variable 'ArchiveStorageAccountName' not found.");
            var archiveContainerName = Environment.GetEnvironmentVariable("ArchiveForProcessedFilesContainerName") ??
                                    throw new InvalidOperationException(
                                           "Environment Variable 'ArchiveForProcessedFilesContainerName' not found.");

            // Get a client to access the container in the storage account
            var archiveBlobContainerClient = GetBlobContainerClient(log, archiveStName, archiveContainerName);
            
            // Pre-stage the BlobItem
            log.LogInformation($"Creating the BlobClient to create/update the archive file");
            var newArchiveBlobClient = archiveBlobContainerClient.GetBlobClient(newArchiveName);
            // Upload content in the blob Item
            // For various upload source types: https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-upload
            log.LogInformation($"Uploading the Blob and its content");
            var uploadResponse = await newArchiveBlobClient.UploadAsync(BinaryData.FromString(outputContent), overwrite: true);

            log.LogInformation($"Upload result is: {uploadResponse.GetRawResponse().ReasonPhrase}");

            // Push the file to Bank X SFTP


            // Log processing details


            // Delete the files processed in Drop



            log.LogInformation($"C# Timer triggered function InputFilesProcessor FINISHED at: {DateTime.Now}");
        }

        private static async Task<string> ProcessDropBlobItemAsync(ILogger log, BlobItem blobItem, MemoryStream memStream)
        {
            log.LogInformation($"Processing the blob {blobItem.Name} content");

            var output = string.Empty;

            // Adding BlobItem based info
            var blobItemData = $"BlobItem   infos: Name={blobItem.Name}, Size={blobItem.Properties.ContentLength}, Type={blobItem.Properties.BlobType}";
            log.LogInformation($"Adding BlobItem info ({blobItemData})");
            output += blobItemData + "\r\n";

            // Adding content based info
            memStream.Position = 0; // Ensuring stream is at position 0 before reading its content.
            var reader = new StreamReader(memStream);
            var content = await reader.ReadToEndAsync();
            var contentData = $"BlobItem content: {content}";
            log.LogInformation($"Adding  Content info ({contentData})");
            output += contentData + "\r\n";
            memStream.Position = 0; // Setting back stream at position 0.

            return output;
        }

        private static async Task LogBlobItemContentAsync(ILogger log, BlobItem blobItem, MemoryStream memStream)
        {
            log.LogInformation($"Content of Blob {blobItem.Name} is:");
            memStream.Position = 0; // Setting back stream at position 0 to read its content.
            var reader = new StreamReader(memStream);
            log.LogInformation($"  ==> {await reader.ReadToEndAsync()} <==");
            memStream.Position = 0; // Setting back stream at position 0.
        }

        private static BlobClient GetBlobClient(BlobContainerClient blobContainerClient, BlobItem blobItem)
        {
            return blobContainerClient.GetBlobClient(blobItem.Name);
        }

        private static void LogFoundBlobItem(ILogger log, BlobItem blobItem)
        {
            var blobType = "  File";
            if (blobItem.Properties.ContentLength == 0)
            {
                blobType = "Folder";
            }

            log.LogInformation("   " + blobType + ": " + blobItem.Name);
        }

        private static AsyncPageable<BlobItem> GetBlobItemsAsync(BlobContainerClient blobContainerClient)
        {
            // Get blobs' list in container
            var blobsList = blobContainerClient.GetBlobsAsync();
            return blobsList;
        }

        private static BlobContainerClient GetBlobContainerClient(ILogger log, string stAccountName, string containerName)
        {
            log.LogInformation($"Creating a BlobContainerClient for: {stAccountName}/{containerName}");
            // Connecting to Storage Account / Blob
            var blobServiceClient = new BlobServiceClient(
                new Uri($"https://{stAccountName}.blob.core.windows.net"),
                new DefaultAzureCredential());

            // Access container
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            return blobContainerClient;
        }

        //public static async Task<MemoryStream> ToMemoryStreamAsync(Stream stream)
        //{
        //    var memStream = stream as MemoryStream;
        //    if (memStream == null)
        //    {
        //        memStream = new MemoryStream();
        //        memStream.Flush();
        //        memStream.Position = 0;
        //        await stream.CopyToAsync(memStream);
        //    }
        //    return memStream;
        //}

    }
}
