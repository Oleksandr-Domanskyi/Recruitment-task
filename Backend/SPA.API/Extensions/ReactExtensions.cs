
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SPA.API.Extensions;

public static class ReactExtensions
{
    private const string CorsPolicy = "ReactClient";

    public static IServiceCollection AddReactCors(
        this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, policy =>
            {
                policy
                    .WithOrigins("https://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseReactCors(
        this IApplicationBuilder app)
    {
        app.UseCors(CorsPolicy);

        return app;
    }
}
