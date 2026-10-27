using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comment.Application.DTO.ReadModels;
using FluentResults;
using MediatR;

namespace Comment.Application.CQRS.Query.GetUserDiscussions
{
    public sealed record GetUserDiscussionsQuery(
        Guid UserId,
        int Limit = 5
    ) : IRequest<Result<List<UserDiscussionDto>>>;
}