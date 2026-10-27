using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace User.Infrastructure.Database;

public sealed class UserDbContextFactory
    : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "ConnectionStrings__DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "ConnectionStrings__DefaultConnection is missing.");

        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new UserDbContext(options);
    }
}