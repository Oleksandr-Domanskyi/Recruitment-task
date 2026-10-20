using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SPA.Database.Shared.Database;

public abstract class SPAIdentityDbContext<TUser>
    : IdentityDbContext<TUser, IdentityRole<Guid>, Guid>
    where TUser : IdentityUser<Guid>
{
    protected abstract string Schema { get; }

    protected SPAIdentityDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
    }
}

public abstract class SPADbContext : DbContext
{
    protected abstract string Schema { get; }

    protected SPADbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
    }
}