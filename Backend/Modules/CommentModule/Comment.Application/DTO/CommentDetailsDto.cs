using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Application.DTO
{
    public sealed record CommentDetailsDto(
        Guid Id,
        Guid? UserId,
        string Text,
        string? ImageUrl,
        int Rating,
        DateTime CreatedAt,
        Guid? ParentCommentId,
        IReadOnlyList<CommentDetailsDto> Replies
    );
}