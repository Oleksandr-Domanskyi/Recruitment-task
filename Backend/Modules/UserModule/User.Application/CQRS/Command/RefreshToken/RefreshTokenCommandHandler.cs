
using FluentResults;
using MediatR;
using User.Application.DTO.Responses;
using User.Application.Interfaces;

namespace User.Application.CQRS.Command.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenService refreshTokenService)
    : IRequestHandler<RefreshTokenCommand, Result<AuthTokenResponse>>
{
    public async Task<Result<AuthTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        return await refreshTokenService.RefreshAsync(
            request.RefreshToken,
            cancellationToken);
    }
}
