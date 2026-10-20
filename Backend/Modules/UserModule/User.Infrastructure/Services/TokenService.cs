
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using User.Application.Interfaces;
using User.Core.Entity;
using User.Infrastructure.Options;

namespace User.Infrastructure.Services;

public sealed class TokenService(
    UserManager<UserEntity> userManager,
    IOptions<JwtOptions> options)
    : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    public async Task<string> GenerateAccessTokenAsync(
        UserEntity user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        claims.AddRange(
            roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.Secret));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(
            _options.AccessTokenMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(refreshToken));

        return Convert.ToHexString(bytes);
    }
}
