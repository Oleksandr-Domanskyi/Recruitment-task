using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comment.Application.DTO
{
    public sealed record PagedCommentsDto(
        IReadOnlyList<CommentDto> Items,
        int Page,
        int PageSize,
        int TotalCount,
        int TotalPages
    );
}