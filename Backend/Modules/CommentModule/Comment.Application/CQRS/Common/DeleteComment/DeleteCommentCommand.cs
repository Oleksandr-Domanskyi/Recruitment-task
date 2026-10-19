using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Common.DeleteComment
{
    public sealed record DeleteCommentCommand(Guid CommentId) : IRequest<Result>;

}