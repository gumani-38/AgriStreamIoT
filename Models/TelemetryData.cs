
using Azure; // Add this line
using Azure.Data.Tables;

namespace AgriStreamIoT.Models
{
    public class TelemetryData : ITableEntity
    {
        public string PartitionKey { get; set; } = "FarmSensor"; // e.g., FarmId
        public string RowKey { get; set; } = Guid.NewGuid().ToString(); // Unique Entry ID
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string SensorId { get; set; } = string.Empty;
        public double SoilMoisture { get; set; }
        public double Temperature { get; set; }
        public string EnvironmentalStatus { get; set; } = "Normal";
    }
}
