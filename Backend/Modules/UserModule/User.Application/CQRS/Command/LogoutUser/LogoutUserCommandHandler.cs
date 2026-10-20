
using FluentResults;
using MediatR;
using User.Application.Interfaces;

namespace User.Application.CQRS.Command.LogoutUser;

public sealed class LogoutUserCommandHandler(
    IRefreshTokenService refreshTokenService)
    : IRequestHandler<LogoutUserCommand, Result>
{
    public async Task<Result> Handle(
        LogoutUserCommand request,
        CancellationToken cancellationToken)
    {
        return await refreshTokenService.RevokeAsync(
            request.RefreshToken,
            request.userId,
            cancellationToken);
    }
}
