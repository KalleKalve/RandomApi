using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DataFetcher;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

public static class FetchDataFunction
{
    private static readonly HttpClient httpClient = new HttpClient();

    [FunctionName("FetchDataFunction")]
    public static async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
    {
        string apiUrl = Environment.GetEnvironmentVariable(ConfigString.ApiUrl);
        string storageConnectionString = Environment.GetEnvironmentVariable(ConfigString.StorageConnectionStringKey);
        string tableName = Environment.GetEnvironmentVariable(ConfigString.TableNameKey);
        string blobContainerName = Environment.GetEnvironmentVariable(ConfigString.BlobContainerNameKey);

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
            PartitionKey = ConfigString.PartitionKey,
            RowKey = Guid.NewGuid().ToString(),
            [ConfigString.Success] = isSuccess,
            [ConfigString.Timestamp] = DateTime.UtcNow,
            [ConfigString.BlobUrl] = blobUrl
        };

        await tableClient.AddEntityAsync(logEntity);
    }
}

