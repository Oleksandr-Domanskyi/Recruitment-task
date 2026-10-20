
using FluentResults;
using Microsoft.Extensions.Options;
using User.Application.DTO.Responses;
using User.Application.Interfaces;
using User.Core.Entity;
using User.Infrastructure.Options;
using User.Infrastructure.Repositories.IRepositories;

namespace User.Infrastructure.Services;

public sealed class RefreshTokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepositoryService userRepositoryService,
    ITokenService tokenService,
    IOptions<JwtOptions> options)
    : IRefreshTokenService
{
    private readonly JwtOptions _options = options.Value;

    public async Task<Result<AuthTokenResponse>> CreateSessionAsync(
        UserEntity user,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var refreshToken = tokenService.GenerateRefreshToken();

        var entity = new RefreshTokenEntity(
            user.Id,
            tokenService.HashRefreshToken(refreshToken),
            now.AddDays(_options.RefreshTokenDays));

        var accessToken = await tokenService.GenerateAccessTokenAsync(
            user,
            cancellationToken);

        await refreshTokenRepository.SaveAsync(
            entity,
            cancellationToken);

        return Result.Ok(new AuthTokenResponse(
            accessToken,
            refreshToken,
            now.AddMinutes(_options.AccessTokenMinutes)));
    }

    public async Task<Result<AuthTokenResponse>> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result.Fail<AuthTokenResponse>("Invalid refresh token.");

        var tokenHash = tokenService.HashRefreshToken(refreshToken);

        var token = await refreshTokenRepository.GetValidAsync(
            tokenHash,
            cancellationToken);

        if (token is null)
            return Result.Fail<AuthTokenResponse>(
                "Invalid or expired refresh token.");

        var userResult = await userRepositoryService.GetByIdAsync(
            token.UserId,
            cancellationToken);

        if (userResult.IsFailed)
            return Result.Fail<AuthTokenResponse>(userResult.Errors);

        var user = userResult.Value;
        var now = DateTime.UtcNow;

        var newRefreshToken = tokenService.GenerateRefreshToken();

        var newEntity = new RefreshTokenEntity(
            user.Id,
            tokenService.HashRefreshToken(newRefreshToken),
            now.AddDays(_options.RefreshTokenDays));

        var accessToken = await tokenService.GenerateAccessTokenAsync(
            user,
            cancellationToken);

        var rotated = await refreshTokenRepository.RotateAsync(
            token.Id,
            newEntity,
            cancellationToken);

        if (!rotated)
            return Result.Fail<AuthTokenResponse>(
                "Refresh token already used or expired.");

        return Result.Ok(new AuthTokenResponse(
            accessToken,
            newRefreshToken,
            now.AddMinutes(_options.AccessTokenMinutes)));
    }

    public async Task<Result> RevokeAsync(
        string refreshToken,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result.Fail("Invalid refresh token.");

        var tokenHash = tokenService.HashRefreshToken(refreshToken);

        var revoked = await refreshTokenRepository.RevokeAsync(
            tokenHash,
            userId,
            cancellationToken);

        return revoked
            ? Result.Ok()
            : Result.Fail("Refresh token not found or already revoked.");
    }
}
