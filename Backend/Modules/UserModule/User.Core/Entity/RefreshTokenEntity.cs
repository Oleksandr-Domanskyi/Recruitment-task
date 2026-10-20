namespace User.Core.Entity;

public sealed class RefreshTokenEntity(
    Guid userId,
    string tokenHash,
    DateTime expiresAt)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; } = userId;

    public string TokenHash { get; private set; } = tokenHash;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; private set; } = expiresAt;
    public DateTime? RevokedAt { get; private set; }
    public UserEntity User { get; private set; } = null!;
}