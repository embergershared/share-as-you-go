using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class DroppedFileProcessing
    {
        [FunctionName("DroppedFileProcessing")]
        public void Run([BlobTrigger("sftp/{name}", Connection = "DropST")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"\nC# Function DroppedFileProcessing was triggered by addition of a new Blob in Drop Storage Account:\n   - Name: {name} \n   - Size: {myBlob.Length} Bytes");

            log.LogInformation($"Starting processing of \"{name}\"");

            // Getting the name of Archive ST from Env variables
            var archiveStName = Environment.GetEnvironmentVariable("ArchiveStorageAccountName");
            var archiveContainerName = Environment.GetEnvironmentVariable("ArchiveContainerSaveName");

            log.LogInformation($"\nStarting move of the file to:\n   - Storage account: {archiveStName}\n   - Container      : {archiveContainerName}");



        }
    }
}
