using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Application.DTO.ReadModels
{
    public sealed record PagedCommentEntities(
        IReadOnlyList<CommentWithRepliesCount> Items,
        int Page,
        int PageSize,
        int TotalCount,
        int TotalPages
    );
}