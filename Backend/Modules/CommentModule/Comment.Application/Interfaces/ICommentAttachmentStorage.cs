using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.DTO;

namespace Comment.Application.Interfaces
{
    public interface ICommentAttachmentStorage
    {
        Task<string> UploadAsync(
            ProcessedCommentFile file,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string blobName,
            string contentType,
            CancellationToken cancellationToken = default);

        string GetBlobUrl(string blobName, string contentType);

    }
}