using AgriStreamIoT.Models;
using Azure;
using Azure.Data.Tables; // Required for ITableEntit
using Microsoft.Extensions.Configuration;

namespace AgriStreamIoT.Service2
{
    public interface IAzureTableService
    {
        Task<List<TelemetryData>> GetLatestTelemetryAsync();
    }

    public class AzureTableService : IAzureTableService
    {
        private readonly TableClient _tableClient;
        private const string TableName = "TelemetryData";

        public AzureTableService(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("AzureStorageConnectionString")
                ?? throw new InvalidOperationException("AzureStorageConnectionString is not configured.");

            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient(TableName);
            _tableClient.CreateIfNotExists();
        }

        public async Task<List<TelemetryData>> GetLatestTelemetryAsync()
        {
            var telemetryList = new List<TelemetryData>();

            await foreach (var entity in _tableClient.QueryAsync<TelemetryData>())
            {
                telemetryList.Add(entity);
            }

            return telemetryList.OrderByDescending(t => t.Timestamp).ToList();
        }
    }
}