using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BookShop.BlobStorage
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobService(BlobServiceClient blobServiceClient, string containerName)
        {
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string? fileName = null)
        {
            string name = fileName ?? $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            
            var blobClient = _containerClient.GetBlobClient(name);
            
            var blobHttpHeaders = new BlobHttpHeaders 
            { 
                ContentType = file.ContentType 
            };
            
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, blobHttpHeaders);
            
            return blobClient.Uri.ToString();
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}