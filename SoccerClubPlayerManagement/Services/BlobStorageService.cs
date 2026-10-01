using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SoccerClubPlayerManagement.Services
{
    public class BlobStorageService
    {
        private const string ContainerName = "player-photos";
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage")
                ?? "UseDevelopmentStorage=true"; // Azurite fallback for local dev, matching course-enrolment setup

            var serviceClient = new BlobServiceClient(connectionString);
            _containerClient = serviceClient.GetBlobContainerClient(ContainerName);

            // Public read access so <img src="..."> tags can load photos directly without a SAS token.
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> UploadPlayerPhotoAsync(IFormFile file)
        {
            var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blobClient = _containerClient.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

            return blobClient.Uri.ToString();
        }

        public async Task DeletePlayerPhotoAsync(string photoUrl)
        {
            // Photos saved before the switch to blob storage have relative paths like
            // "/uploads/players/abc.jpg". Those aren't blobs, so there's nothing to delete here.
            if (!Uri.TryCreate(photoUrl, UriKind.Absolute, out var uri))
                return;

            // Extract the blob name from the full URL so we can delete the old photo when replaced.
            var blobName = Path.GetFileName(uri.LocalPath);
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}
