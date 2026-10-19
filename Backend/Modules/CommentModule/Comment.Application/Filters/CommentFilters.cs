using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.Filters.Enums;

namespace Comment.Application.Filters
{
    public class CommentFilters
    {
        public const int PageSize = 25;

        public int Page { get; init; } = 1;
        public CommentSortBy SortBy { get; init; } = CommentSortBy.CreatedAt;
        public SortDirection Direction { get; init; } = SortDirection.Desc;
    }
}