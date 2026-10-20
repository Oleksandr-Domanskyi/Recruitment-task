
using FluentResults;
using MediatR;
using User.Application.DTO.Responses;

namespace User.Application.CQRS.Command.UserLogin;

public sealed record UserLoginCommand(string Email, string Password) : IRequest<Result<AuthTokenResponse>>;
