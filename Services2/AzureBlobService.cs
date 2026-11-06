using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AgriStreamIoT.Service2
{
    public interface IAzureBlobService
    {
        Task<string> UploadDroneImageAsync(IFormFile file);
    }

    public class AzureBlobService : IAzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "drone-images";

        public AzureBlobService(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("AzureStorageConnectionString")
                ?? throw new InvalidOperationException("AzureStorageConnectionString is not configured.");

            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadDroneImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or invalid.", nameof(file));

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return blobClient.Uri.ToString();
        }
    }
}