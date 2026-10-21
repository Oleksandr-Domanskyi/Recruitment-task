using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.DTO;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Common.UpdateComment
{
    public sealed record UpdateCommentCommand(Guid CommentId, UpdateCommentRequest UpdateCommentRequest) : IRequest<Result<CommentDto>>;
}