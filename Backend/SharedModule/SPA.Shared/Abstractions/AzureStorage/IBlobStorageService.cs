using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPA.Shared.Abstractions.AzureStorage
{

    public interface IBlobStorageService
    {
        Task<string> UploadAsync(
            Stream content,
            string containerName,
            string blobName,
            string contentType,
            CancellationToken cancellationToken = default);

        Task<Stream> DownloadAsync(
            string containerName,
            string blobName,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string containerName,
            string blobName,
            CancellationToken cancellationToken = default);
    }
}