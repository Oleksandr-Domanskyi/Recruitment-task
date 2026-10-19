using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.DTO;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Query.GetRepliesComment
{
    public sealed record GetRepliesCommentsQuery(Guid ParentCommentId)
        : IRequest<Result<List<CommentDto>>>;
}