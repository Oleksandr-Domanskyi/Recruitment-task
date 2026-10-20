
using Microsoft.OpenApi;

namespace SPA.API.Extensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwaggerConfiguration(
        this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT access token."
            });

            options.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        "Bearer",
                        document)] = []
                });
        });

        return services;
    }
}
