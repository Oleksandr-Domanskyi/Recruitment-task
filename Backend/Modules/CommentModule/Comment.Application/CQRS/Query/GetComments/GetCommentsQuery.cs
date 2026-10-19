using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.DTO;
using Comment.Application.Filters;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Query.GetComments
{
    public sealed record GetCommentsQuery(CommentFilters Filter) : IRequest<Result<PagedCommentsDto>>;
}