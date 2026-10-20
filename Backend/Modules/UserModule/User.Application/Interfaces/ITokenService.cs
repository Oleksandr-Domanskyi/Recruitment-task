
using User.Core.Entity;

namespace User.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(
        UserEntity user,
        CancellationToken cancellationToken = default);

    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}
