
using FluentResults;
using User.Application.DTO.Responses;
using User.Core.Entity;

namespace User.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<Result<AuthTokenResponse>> CreateSessionAsync(
        UserEntity user,
        CancellationToken cancellationToken = default);

    Task<Result<AuthTokenResponse>> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result> RevokeAsync(
        string refreshToken,
        Guid userId,
        CancellationToken cancellationToken = default);
}
