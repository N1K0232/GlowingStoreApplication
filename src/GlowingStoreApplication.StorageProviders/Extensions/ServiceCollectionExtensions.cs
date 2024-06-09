using Microsoft.Extensions.DependencyInjection;

namespace GlowingStoreApplication.StorageProviders;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureStorage(this IServiceCollection services, Action<AzureStorageOptions> configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        var options = new AzureStorageOptions();
        configuration.Invoke(options);

        services.AddSingleton(options);
        services.AddScoped<IStorageProvider, AzureStorageProvider>();

        return services;
    }

    public static IServiceCollection AddFileSystemStorage(this IServiceCollection services, Action<FileSystemStorageOptions> configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        var options = new FileSystemStorageOptions();
        configuration.Invoke(options);

        services.AddSingleton(options);
        services.AddScoped<IStorageProvider, FileSystemStorageProvider>();

        return services;
    }
}