using FluentResults;
using MediatR;
using User.Application.DTO.Responses;

namespace User.Application.CQRS.Command.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthTokenResponse>>;