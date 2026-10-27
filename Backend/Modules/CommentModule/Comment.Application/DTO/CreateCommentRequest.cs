
using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Application.DTO;

public sealed record CreateCommentRequest(
    Guid? UserId,
    string? UserName,
    string Text,
    Guid? ParentCommentId,
    Stream? File,
    string? FileName,
    Guid CaptchaId,
    string CaptchaAnswer
)
{
    public CommentEntity ToEntity(string safeText)
    {
        return new CommentEntity(
            text: safeText,
            userId: UserId,
            userName: UserName,
            parentCommentId: ParentCommentId
        );
    }
}
