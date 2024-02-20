using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

public static class FetchDataFunction
{
    private static readonly HttpClient httpClient = new HttpClient();

    [FunctionName("FetchDataFunction")]
    public static async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
    {
        string apiUrl = "https://api.publicapis.org/random?auth=null";

        string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        string tableName = Environment.GetEnvironmentVariable("LogTableName");
        string blobContainerName = Environment.GetEnvironmentVariable("FetchedPayloads");

        try
        {
            var response = await httpClient.GetStringAsync(apiUrl);
            log.LogInformation($"Fetched data: {response}");

            var blobServiceClient = new BlobServiceClient(storageConnectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            await blobContainerClient.CreateIfNotExistsAsync(publicAccessType: PublicAccessType.Blob);
            var blobClient = blobContainerClient.GetBlobClient($"Payload-{DateTime.UtcNow:yyyyMMddHHmmss}.json");
            await blobClient.UploadAsync(BinaryData.FromString(response), overwrite: true);

            await LogAttemptToTable(storageConnectionString, tableName, true, blobClient.Uri.ToString());
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to fetch data: {ex.Message}");
            await LogAttemptToTable(storageConnectionString, tableName, false, null);
        }
    }

    private static async Task LogAttemptToTable(string connectionString, string tableName, bool isSuccess, string blobUrl)
    {
        var tableClient = new TableClient(connectionString, tableName);
        await tableClient.CreateIfNotExistsAsync();

        var logEntity = new TableEntity
        {
            PartitionKey = "FetchAttempt",
            RowKey = Guid.NewGuid().ToString(),
            ["Success"] = isSuccess,
            ["Timestamp"] = DateTime.UtcNow,
            ["BlobUrl"] = blobUrl
        };

        await tableClient.AddEntityAsync(logEntity);
    }
}

