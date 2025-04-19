using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace capyborrowProject.Service
{
    public class BlobStorageService(IConfiguration configuration)
    {
        private readonly BlobServiceClient _blobServiceClient = new(configuration["AzureBlobStorage:ConnectionString"]);
        private readonly string _assignmentFilesContainerName = configuration["AzureBlobStorage:AssignmentFilesContainerName"]!;
        private readonly string _submissionFilesContainerName = configuration["AzureBlobStorage:SubmissionFilesContainerName"]!;
        private readonly string _profilePicturesContainerName = configuration["AzureBlobStorage:ProfilePicturesContainerName"]!;

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string fileType, string contentType)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(fileType == "assignment" ? _assignmentFilesContainerName : _submissionFilesContainerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = blobContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

            return blobClient.Uri.ToString();
        }

        public async Task<string> UploadProfilePictureAsync(IFormFile file, string userId)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_profilePicturesContainerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = blobContainerClient.GetBlobClient($"{userId}/{Guid.NewGuid()}_{file.Name}");

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

            return blobClient.Uri.ToString();
        }

        public async Task<bool> DeleteProfilePictureAsync(string profilePictureUrl)
        {
            if (string.IsNullOrEmpty(profilePictureUrl) || !Uri.TryCreate(profilePictureUrl, UriKind.Absolute, out var uri))
                return false;

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_profilePicturesContainerName);

            var blobName = string.Join("/", uri.Segments.Skip(2).Select(s => s.Trim('/')));

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<Stream?> DownloadFileAsync(string fileName, string fileType)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(fileType == "assignment" ? _assignmentFilesContainerName : _submissionFilesContainerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                var downloadInfo = await blobClient.DownloadAsync();
                return downloadInfo.Value.Content;
            }

            return null;
        }

        public async Task<bool> DeleteFileAsync(string fileName, string fileType)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(fileType == "assignment" ? _assignmentFilesContainerName : _submissionFilesContainerName);
            
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            return await blobClient.DeleteIfExistsAsync();
        }
    }
}
