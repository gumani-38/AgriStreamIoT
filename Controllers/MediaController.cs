using AgriStreamIoT.Models;
using AgriStreamIoT.Service2; // References your Service2 folder namespace
using Microsoft.AspNetCore.Mvc;

namespace AgriStreamIoT.Controllers
{
    public class MediaController : Controller
    {
        private readonly IAzureBlobService _blobService;

        public MediaController(IAzureBlobService blobService)
        {
            _blobService = blobService;
        }

        // GET: /Media/Upload
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        // POST: /Media/Upload
        [HttpPost]
        public async Task<IActionResult> Upload(DroneUploadViewModel model)
        {
            if (ModelState.IsValid && model.ImageFile != null)
            {
                try
                {
                    // Upload image to Azure Blob Storage via the service
                    string blobUrl = await _blobService.UploadDroneImageAsync(model.ImageFile);

                    ViewBag.BlobUrl = blobUrl;
                    TempData["SuccessMessage"] = "Drone image successfully uploaded to Azure Blob Storage!";
                    return View();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Upload failed: {ex.Message}");
                }
            }

            return View(model);
        }
    }
}