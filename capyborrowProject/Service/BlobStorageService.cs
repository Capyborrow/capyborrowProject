using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace capyborrowProject.Service
{
    public class BlobStorageService(IConfiguration configuration)
    {
        private readonly BlobServiceClient _blobServiceClient = new(configuration["AzureBlobStorage:ConnectionString"]);
        private readonly string _containerName = configuration["AzureBlobStorage:ContainerName"]!;

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = blobContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

            return blobClient.Uri.ToString();
        }

        public async Task<Stream?> DownloadFileAsync(string fileName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                var downloadInfo = await blobClient.DownloadAsync();
                return downloadInfo.Value.Content;
            }

            return null;
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);
            return await blobClient.DeleteIfExistsAsync();
        }
    }
}
