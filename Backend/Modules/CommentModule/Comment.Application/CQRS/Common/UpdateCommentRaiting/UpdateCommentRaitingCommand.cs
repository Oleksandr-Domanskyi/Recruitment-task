using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Common.UpdateCommentRaiting
{
    public sealed record UpdateCommentRaitingCommand(Guid ParrentCommentId, int Raiting) : IRequest<Result<int>>;

}