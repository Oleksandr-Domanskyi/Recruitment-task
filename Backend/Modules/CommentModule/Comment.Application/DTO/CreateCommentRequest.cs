
using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Application.DTO;

public sealed record CreateCommentRequest(
    Guid? UserId,
    string Text,
    Guid? ParentCommentId,
    string? ImageUrl
)
{
    public CommentEntity ToEntity(string safeText)
    {
        return new CommentEntity(
            safeText,
            UserId,
            ParentCommentId
        );
    }
}
