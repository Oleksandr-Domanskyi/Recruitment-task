
using AzureStorage.Configuration;
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using Microsoft.Extensions.Options;
using SPA.Shared.Abstractions.AzureStorage;

namespace Comment.Infrastructure.Files;

public sealed class CommentAttachmentStorage(IBlobStorageService blobStorage, IOptions<AzureStorageOptions> options) : ICommentAttachmentStorage
{
    public async Task<string> UploadAsync(ProcessedCommentFile file, CancellationToken cancellationToken = default)
    {
        var container = GetContainer(file.ContentType);
        var blobName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLowerInvariant()}";

        using var stream = new MemoryStream(file.Content);

        await blobStorage.UploadAsync(stream, container, blobName, file.ContentType, cancellationToken);

        return blobName;
    }

    public Task DeleteAsync(string blobName, string contentType, CancellationToken cancellationToken = default)
        => blobStorage.DeleteAsync(GetContainer(contentType), blobName, cancellationToken);

    private string GetContainer(string contentType)
        => contentType == CommentFileConstants.TextContentType
            ? options.Value.CommentFilesContainer
            : options.Value.CommentImagesContainer;
}
