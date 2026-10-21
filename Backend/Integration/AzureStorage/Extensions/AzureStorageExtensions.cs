using Azure.Storage.Blobs;
using AzureStorage.Configuration;
using AzureStorage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SPA.Shared.Abstractions.AzureStorage;

namespace AzureStorage.Extensions;

public static class AzureStorageExtensions
{
    public static IServiceCollection AddAzureStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(
            AzureStorageOptions.SectionName);

        services.Configure<AzureStorageOptions>(section);

        var connectionString = section["ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Azure Storage connection string is not configured.");
        }

        services.AddSingleton(new BlobServiceClient(connectionString));
        services.AddScoped<IBlobStorageService, AzureBlobStorageService>();

        return services;
    }
}