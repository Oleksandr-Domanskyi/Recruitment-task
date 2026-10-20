using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using MediatR;

namespace User.Application.CQRS.Command.LogoutUser
{
    public sealed record LogoutUserCommand(string RefreshToken, Guid userId) : IRequest<Result>;
}