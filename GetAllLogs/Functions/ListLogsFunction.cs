using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;

namespace DataFetcher.Functions
{
    public static class ListLogsFunction
    {
        [FunctionName("ListLogs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string from = req.Query[ConfigString.From];
            string to = req.Query[ConfigString.To];
            string connectionString = Environment.GetEnvironmentVariable(ConfigString.StorageConnectionStringKey);
            string tableName = Environment.GetEnvironmentVariable(ConfigString.TableNameKey);

            var tableClient = new TableClient(connectionString, tableName);
            await tableClient.CreateIfNotExistsAsync();

            DateTime fromDate;
            DateTime toDate;
            if (!DateTime.TryParse(from, out fromDate) || !DateTime.TryParse(to, out toDate))
            {
                return new BadRequestObjectResult("Please pass a valid 'from' and 'to' date in the query string.");
            }

            var filter = TableClient.CreateQueryFilter($"Timestamp ge {fromDate:o} and Timestamp le {toDate:o}");
            var queryResults = tableClient.Query<TableEntity>(filter);

            return new OkObjectResult(queryResults);
        }
    }
}
