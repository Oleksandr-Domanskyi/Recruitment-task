
using Comment.Application.DTO;
using Comment.Application.Interfaces;
using Comment.Application.Mappers;
using Comment.Core.Entity;
using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Application.Services;

public sealed class CommentAttachmentManager : ICommentAttachmentManager
{
    private readonly ICommentFileProcessor _fileProcessor;
    private readonly ICommentAttachmentStorage _attachmentStorage;

    public CommentAttachmentManager(ICommentFileProcessor fileProcessor, ICommentAttachmentStorage attachmentStorage)
    {
        _fileProcessor = fileProcessor;
        _attachmentStorage = attachmentStorage;
    }

    public async Task<CommentAttachment?> UploadAsync(CommentEntity comment, Stream? stream, string? fileName, CancellationToken cancellationToken)
    {
        if (stream is null)
            return null;

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.");

        var file = await _fileProcessor.ProcessAsync(stream, fileName, cancellationToken);
        var blobName = await _attachmentStorage.UploadAsync(file, cancellationToken);
        var attachment = CommentAttachmentMapper.ToEntity(comment.Id, blobName, file);

        comment.AddAttachment(attachment);
        return attachment;
    }

    public Task DeleteAsync(CommentAttachment? attachment, CancellationToken cancellationToken)
    {
        return attachment is null
            ? Task.CompletedTask
            : _attachmentStorage.DeleteAsync(attachment.BlobName, attachment.ContentType, cancellationToken);
    }

    public async Task<CommentAttachment?> UpdateAsync(CommentEntity comment, UpdateCommentRequest request, CancellationToken cancellationToken)
    {
        if (request.File is null && !request.RemoveAttachment)
            return null;

        if (request.File is not null && request.RemoveAttachment)
            throw new ArgumentException("Cannot upload and remove an attachment simultaneously.");

        var oldAttachment = comment.Attachments.FirstOrDefault();

        if (request.File is not null)
        {
            await UploadAsync(comment, request.File, request.FileName, cancellationToken);

            if (oldAttachment is not null)
                comment.RemoveAttachment(oldAttachment);
        }
        else if (oldAttachment is not null)
        {
            comment.RemoveAttachment(oldAttachment);
        }

        return oldAttachment;
    }
}
