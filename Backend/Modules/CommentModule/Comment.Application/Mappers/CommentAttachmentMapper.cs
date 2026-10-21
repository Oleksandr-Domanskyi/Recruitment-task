
using Comment.Application.DTO;
using Comment.Core.Entity;
using Comment.Core.Entity.Enums;

namespace Comment.Application.Mappers;

public static class CommentAttachmentMapper
{
    public static CommentAttachment ToEntity(Guid commentId, string blobName, ProcessedCommentFile file)
    => new CommentAttachment(
        commentId,
        blobName,
        file.FileName,
        file.ContentType,
        file.Content.LongLength,
        file.ContentType == "text/plain"
            ? AttachmentType.TextFile
            : AttachmentType.Image);

}
