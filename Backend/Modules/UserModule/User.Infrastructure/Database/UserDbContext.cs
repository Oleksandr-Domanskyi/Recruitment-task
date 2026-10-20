using Microsoft.EntityFrameworkCore;
using SPA.Database.Shared.Database;
using User.Core.Entity;

namespace User.Infrastructure.Database;

public class UserDbContext(DbContextOptions<UserDbContext> options)
    : SPAIdentityDbContext<UserEntity>(options)
{
    protected override string Schema => "users";

    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(UserDbContext).Assembly);
    }
}