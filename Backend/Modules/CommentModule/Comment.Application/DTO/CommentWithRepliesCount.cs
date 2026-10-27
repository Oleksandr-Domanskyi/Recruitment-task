
using CommentEntity = Comment.Core.Entity.Comment;

namespace Comment.Application.DTO;

public sealed record CommentWithRepliesCount(
    CommentEntity Comment,
    int RepliesCount,
    short UserVote = 0,
    string? UserName = null,
    string? AvatarBlobName = null
);
