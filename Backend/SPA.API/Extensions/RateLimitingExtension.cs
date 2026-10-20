
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using SPA.API.Constants;
using SPA.Shared.Constants;

namespace SPA.API.Extensions;

public static class RateLimitingExtension
{
    public static IServiceCollection AddRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode =
                StatusCodes.Status429TooManyRequests;

            options.AddPolicy(RateLimitPolicies.Comments, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: GetClientKey(context),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.AddPolicy(RateLimitPolicies.Auth, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: GetClientKey(context),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(5),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
        });

        return services;
    }

    private static string GetClientKey(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
    }
}
