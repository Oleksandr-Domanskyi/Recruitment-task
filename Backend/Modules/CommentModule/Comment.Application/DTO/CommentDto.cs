
using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Application.DTO;

public sealed record CommentDto(
    Guid Id,
    Guid? UserId,
    string Text,
    string? ImageUrl,
    int Rating,
    DateTime CreatedAt,
    Guid? ParentCommentId,
    int RepliesCount
)
{
    public CommentDto(CommentEntity entity, int repliesCount = 0)
        : this(
            entity.Id,
            entity.UserId,
            entity.Text,
            entity.ImageUrl,
            entity.Rating,
            entity.CreatedAt,
            entity.ParentCommentId,
            repliesCount)
    { }
}
