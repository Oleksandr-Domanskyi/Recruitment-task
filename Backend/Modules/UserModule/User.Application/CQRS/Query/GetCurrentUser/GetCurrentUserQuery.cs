using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using User.Application.DTO.Responses;

namespace User.Application.CQRS.Query.GetCurrentUser
{
    public sealed record GetCurrentUserQuery(Guid UserId)
        : IRequest<Result<UserResponse>>;
}