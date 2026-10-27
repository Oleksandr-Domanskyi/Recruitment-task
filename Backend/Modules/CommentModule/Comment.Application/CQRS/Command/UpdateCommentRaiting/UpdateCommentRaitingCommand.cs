using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Command.UpdateCommentRaiting
{
    public sealed record UpdateCommentRatingCommand(
        Guid CommentId,
        Guid UserId,
        short Value
    ) : IRequest<Result<int>>;
}