
using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Application.DTO
{
    public sealed record CommentWithRepliesCount(CommentEntity Comment, int RepliesCount);
}