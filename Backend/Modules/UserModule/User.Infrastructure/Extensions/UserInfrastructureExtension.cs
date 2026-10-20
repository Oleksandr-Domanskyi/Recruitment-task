
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User.Application.Interfaces;
using User.Core.Entity;
using User.Infrastructure.Database;
using User.Infrastructure.Options;
using User.Infrastructure.Repositories;
using User.Infrastructure.Repositories.IRepositories;
using User.Infrastructure.Services;

namespace User.Infrastructure.Extensions;

public static class UserInfrastructureExtension
{
    public static void AddUserInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentityCore<UserEntity>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddSignInManager()
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IUserRepositoryService, UserRepositoryServices>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
    }
}
