using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using Azure.Storage.Blobs;

namespace DataFetcher.Functions
{
    public static class FetchPayloadFunction
    {
        [FunctionName("FetchPayload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string rowKey = req.Query["rowKey"];
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string tableName = Environment.GetEnvironmentVariable("LogTableName");

            if (string.IsNullOrEmpty(rowKey))
            {
                return new BadRequestObjectResult("Please pass a 'rowKey' in the query string.");
            }

            var tableClient = new TableClient(connectionString, tableName);
            await tableClient.CreateIfNotExistsAsync();

            var entity = await tableClient.GetEntityAsync<TableEntity>("FetchAttempt", rowKey);

            if (entity == null || !entity.Value.ContainsKey("BlobUrl"))
            {
                return new NotFoundResult();
            }

            string blobUrl = entity.Value["BlobUrl"].ToString();
            var blobClient = new BlobClient(new Uri(blobUrl));
            var downloadContent = await blobClient.DownloadContentAsync();

            return new OkObjectResult(downloadContent.Value.Content.ToString());
        }
    }
}
