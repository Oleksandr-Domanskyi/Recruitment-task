
using User.Core.Entity;

namespace User.Application.DTO.Responses;

public sealed record UserResponse(
    Guid Id,
    string UserName,
    string Email,
    string? AvatarUrl,
    DateTime CreatedAt)
{
    public UserResponse(
        UserEntity user,
        Func<string, string> getAvatarUrl)
        : this(
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            string.IsNullOrWhiteSpace(user.AvatarUrl)
                ? null
                : getAvatarUrl(user.AvatarUrl),
            user.CreatedAt)
    {
    }
}
