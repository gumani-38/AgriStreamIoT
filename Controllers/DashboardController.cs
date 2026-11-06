using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Azure.Data.Tables;
using System.Text.Json;
using System.Text;
using AgriStreamIoT.Models;
using AgriStreamIoT.Service2;

namespace AgriStreamIoT.Controllers
{
    [Authorize] // Protects the dashboard so only logged-in users can access it
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AzureTableService _tableService;

        public DashboardController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            AzureTableService tableService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _tableService = tableService;
        }

        // GET: /Dashboard/Index
        public async Task<IActionResult> Index()
        {
            string connectionString =
                _configuration.GetConnectionString("AzureStorageConnectionString")
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string is missing from appsettings.json."
                );
            var tableClient = new TableClient(connectionString, "TelemetryData");
            await tableClient.CreateIfNotExistsAsync();

            var telemetryList = new List<TelemetryData>();

            // Query records from Azure Table Storage
            await foreach (var entity in tableClient.QueryAsync<TelemetryData>())
            {
                telemetryList.Add(entity);
            }

            return View(telemetryList.OrderByDescending(t => t.Timestamp));
        }

        // POST: /Dashboard/Create
        [HttpPost]
        public async Task<IActionResult> Create(string sensorId, double soilMoisture, double temperature, string environmentalStatus)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString("AzureStorageConnectionString")
                    ?? throw new InvalidOperationException(
                        "Azure Storage connection string is missing from appsettings.json."
                    );

                var tableClient = new TableClient(connectionString, "TelemetryData");
                await tableClient.CreateIfNotExistsAsync();

                var newTelemetry = new TelemetryData
                {
                    PartitionKey = "AgriculturalZone",
                    RowKey = Guid.NewGuid().ToString(),
                    SensorId = string.IsNullOrWhiteSpace(sensorId) ? "SENSOR-999" : sensorId,
                    SoilMoisture = soilMoisture,
                    Temperature = temperature,
                    EnvironmentalStatus = string.IsNullOrWhiteSpace(environmentalStatus) ? "Normal" : environmentalStatus,
                    Timestamp = DateTimeOffset.UtcNow
                };

                await tableClient.AddEntityAsync(newTelemetry);
                TempData["Message"] = $"Telemetry record for {newTelemetry.SensorId} created successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to create telemetry record: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Dashboard/TriggerIrrigation
        [HttpPost]
        public async Task<IActionResult> TriggerIrrigation(string sensorId)
        {
            try
            {
                string functionUrl = "https://agristreamfun-bkavceh0fag7dyf3.southafricanorth-01.azurewebsites.net/api/irrigation/trigger";

                var client = _httpClientFactory.CreateClient();
                var payload = new { Command = $"Irrigation Command for Sensor {sensorId}" };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(functionUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = $"Irrigation command successfully sent for sensor {sensorId}!";
                }
                else
                {
                    TempData["Error"] = "Failed to trigger irrigation via Azure Functions.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error connecting to Azure Functions: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}