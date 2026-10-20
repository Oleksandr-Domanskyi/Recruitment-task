
using Microsoft.EntityFrameworkCore;
using User.Core.Entity;
using User.Infrastructure.Database;
using User.Infrastructure.Repositories.IRepositories;

namespace User.Infrastructure.Repositories;

public sealed class RefreshTokenRepository(
    UserDbContext dbContext)
    : IRefreshTokenRepository
{
    public async Task SaveAsync(
        RefreshTokenEntity token,
        CancellationToken cancellationToken = default)
    {
        dbContext.RefreshTokens.Add(token);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshTokenEntity?> GetValidAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash &&
                     x.RevokedAt == null &&
                     x.ExpiresAt > now,
                cancellationToken);
    }

    public async Task<bool> RotateAsync(
        Guid tokenId,
        RefreshTokenEntity newToken,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        var updated = await dbContext.RefreshTokens
            .Where(x =>
                x.Id == tokenId &&
                x.RevokedAt == null &&
                x.ExpiresAt > now)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    x => x.RevokedAt, now),
                cancellationToken);

        if (updated != 1)
            return false;

        dbContext.RefreshTokens.Add(newToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RevokeAsync(
        string tokenHash,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var updated = await dbContext.RefreshTokens
            .Where(x =>
                x.TokenHash == tokenHash &&
                x.UserId == userId &&
                x.RevokedAt == null)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    x => x.RevokedAt, DateTime.UtcNow),
                cancellationToken);

        return updated == 1;
    }
}
