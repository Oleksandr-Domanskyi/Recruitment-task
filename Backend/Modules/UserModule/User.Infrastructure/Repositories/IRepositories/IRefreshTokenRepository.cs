
using User.Core.Entity;

namespace User.Infrastructure.Repositories.IRepositories;

public interface IRefreshTokenRepository
{
    Task SaveAsync(
        RefreshTokenEntity token,
        CancellationToken cancellationToken = default);

    Task<RefreshTokenEntity?> GetValidAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<bool> RotateAsync(
        Guid tokenId,
        RefreshTokenEntity newToken,
        CancellationToken cancellationToken = default);

    Task<bool> RevokeAsync(
        string tokenHash,
        Guid userId,
        CancellationToken cancellationToken = default);
}
