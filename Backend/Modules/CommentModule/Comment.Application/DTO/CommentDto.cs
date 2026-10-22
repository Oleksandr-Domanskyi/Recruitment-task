using Comment.Application.DTO.ReadModels;
using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Application.DTO;

public sealed record CommentDto(
    Guid Id,
    Guid? UserId,
    string Text,
    IReadOnlyList<CommentAttachmentDto> Attachments,
    int Rating,
    DateTime CreatedAt,
    Guid? ParentCommentId,
    int RepliesCount
)
{
    public CommentDto(
        CommentEntity entity,
        Func<string, string, string> getBlobUrl,
        int repliesCount = 0)
        : this(
            entity.Id,
            entity.UserId,
            entity.Text,
           entity.Attachments.Select(x => new CommentAttachmentDto(
            x.Id,
            x.OriginalFileName,
            x.ContentType,
            getBlobUrl(x.BlobName, x.ContentType),
            x.FileSize
        )).ToList(),
            entity.Rating,
            entity.CreatedAt,
            entity.ParentCommentId,
            repliesCount)
    { }
}