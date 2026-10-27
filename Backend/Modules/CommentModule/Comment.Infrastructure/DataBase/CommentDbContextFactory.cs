using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Comment.Infrastructure.DataBase;

public sealed class CommentDbContextFactory
    : IDesignTimeDbContextFactory<CommentDbContext>
{
    public CommentDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "ConnectionStrings__DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "ConnectionStrings__DefaultConnection is missing.");

        var options = new DbContextOptionsBuilder<CommentDbContext>()
            .UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsAssembly(
                    typeof(CommentDbContext).Assembly.FullName))
            .Options;

        return new CommentDbContext(options);
    }
}