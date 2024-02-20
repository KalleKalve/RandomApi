namespace DataFetcher
{
    public static class ConfigString
    {
        // API and Configuration Keys
        public const string ApiUrl = "ApiUrl";
        public const string StorageConnectionStringKey = "AzureWebJobsStorage";
        public const string TableNameKey = "LogTableName";
        public const string BlobContainerNameKey = "PayloadBlobName";
        public const string PartitionKey = "FetchAttempt";

        // Data Entity Keys
        public const string Success = "Success";
        public const string Timestamp = "Timestamp";
        public const string BlobUrl = "BlobUrl";

        // Query strings
        public const string RowKey = "rowKey";
        public const string From = "from";
        public const string To = "to";

    }
}
