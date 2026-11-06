using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AgriStreamIoT.Controllers
{
    [Authorize]
    public class MediaController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _storageConnectionString;
        private readonly string _containerName = "drone-media";

        public MediaController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _storageConnectionString = _configuration.GetConnectionString("AzureStorageConnectionString")
                                      ?? _configuration["AzureWebJobsStorage"]
                                      ?? "UseDevelopmentStorage=true";
        }

        // GET: /Media/Index (Lists files directly from Blob Storage for UI preview)
        public async Task<IActionResult> Index()
        {
            var fileUris = new List<string>();

            try
            {
                var blobServiceClient = new BlobServiceClient(_storageConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                await foreach (var blobItem in containerClient.GetBlobsAsync())
                {
                    var blobClient = containerClient.GetBlobClient(blobItem.Name);
                    fileUris.Add(blobClient.Uri.AbsoluteUri);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error connecting to Azure Blob Storage: {ex.Message}";
            }

            return View(fileUris);
        }

        // POST: /Media/Upload (Forwards the file to your Azure Function)
        // POST: /Media/Upload
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a valid file to upload.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var blobServiceClient = new BlobServiceClient(_storageConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient("drone-images");

                // MUST BE CALLED WITH NO PARAMETERS to prevent the 409 error
                await containerClient.CreateIfNotExistsAsync();

                string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var blobClient = containerClient.GetBlobClient(uniqueFileName);

                using (var stream = file.OpenReadStream())
                {
                    // Uploading here automatically and instantly triggers your DroneImageBlobFunction in the background!
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                TempData["Message"] = $"Successfully uploaded {file.FileName} and triggered drone function!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Upload failed: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}