
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace User.Infrastructure.Database.Seeders;

public static class UserRoleSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        const string roleName = "User";

        if (await roleManager.RoleExistsAsync(roleName))
            return;

        var result = await roleManager.CreateAsync(
            new IdentityRole<Guid>(roleName));

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(x => x.Description));

            throw new InvalidOperationException(
                $"Failed to seed role '{roleName}': {errors}");
        }
    }
}
