
using AgriStreamIoT.Service2;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace AgriStreamIoT.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AzureTableService _tableService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DashboardController(
            AzureTableService tableService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _tableService = tableService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET: Field Dashboard (Retrieves real-time telemetry from Azure Table Storage)
        public async Task<IActionResult> Index()
        {
            var telemetryData = await _tableService.GetLatestTelemetryAsync();
            return View(telemetryData);
        }

        // POST: Trigger Irrigation
        [HttpPost]
        public async Task<IActionResult> TriggerIrrigation(string sensorId)
        {
            if (string.IsNullOrEmpty(sensorId))
            {
                TempData["Error"] = "Invalid Sensor ID provided.";
                return RedirectToAction("Index");
            }

            // Execute the dedicated Azure Function trigger handler
            bool isSuccess = await ExecuteIrrigationTriggerAsync(sensorId, "START_IRRIGATION");

            if (isSuccess)
            {
                TempData["Message"] = $"Irrigation command dispatched successfully for Sensor {sensorId}!";
            }
            else
            {
                TempData["Error"] = $"Failed to dispatch irrigation command for Sensor {sensorId}.";
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Modular Function to dispatch HTTP POST commands to the Azure Function Pipeline
        /// </summary>
        private async Task<bool> ExecuteIrrigationTriggerAsync(string sensorId, string actionCommand)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                string functionUrl = _configuration["AzureFunctions:TriggerIrrigationUrl"]
                    ?? "https://<your-azure-function-app>.azurewebsites.net/api/TriggerIrrigation";

                var payload = new
                {
                    SensorId = sensorId,
                    Command = actionCommand,
                    Timestamp = DateTime.UtcNow
                };

                string jsonContent = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(functionUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log exception details in production environment
                Console.WriteLine($"Error triggering Azure Function: {ex.Message}");
                return false;
            }
        }
    }
}