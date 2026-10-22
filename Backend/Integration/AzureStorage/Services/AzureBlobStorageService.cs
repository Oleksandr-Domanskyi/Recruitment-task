
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SPA.Shared.Abstractions.AzureStorage;
using Azure.Storage.Sas;

namespace AzureStorage.Services;

public sealed class AzureBlobStorageService(BlobServiceClient blobServiceClient) : IBlobStorageService
{
    public async Task<string> UploadAsync(Stream content, string containerName, string blobName, string contentType, CancellationToken cancellationToken = default)
    {
        var blobClient = blobServiceClient.GetBlobContainerClient(containerName).GetBlobClient(blobName);

        await blobClient.UploadAsync(content, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            }
        }, cancellationToken);

        return blobClient.Name;
    }

    public async Task<Stream> DownloadAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        var blobClient = blobServiceClient.GetBlobContainerClient(containerName).GetBlobClient(blobName);

        return await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        var blobClient = blobServiceClient.GetBlobContainerClient(containerName).GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
    public string GetBlobUrl(string containerName, string blobName)
    {
        var blobClient = blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName);

        return blobClient.GenerateSasUri(
            BlobSasPermissions.Read,
            DateTimeOffset.UtcNow.AddHours(1)
        ).ToString();
    }
}
