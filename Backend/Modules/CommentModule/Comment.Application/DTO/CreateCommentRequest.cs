
using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Application.DTO;

public sealed record CreateCommentRequest(
    Guid? UserId,
    string Text,
    Guid? ParentCommentId,
    string? ImageUrl
)
{
    public CommentEntity ToEntity()
    {
        return new CommentEntity(
            Text,
            UserId,
            ParentCommentId
        );
    }
}
