using GrowthBook;
using Pi.Common.Features;
using Pi.SetMarketData.MigrationProxy.Interfaces;
using Pi.SetMarketData.MigrationProxy.Services;

namespace Pi.SetMarketData.MigrationProxy.Startup;

public static class GrowthBookRegistrationExtensions
{
    public static IServiceCollection AddGrowthBookService(this IServiceCollection services, ConfigurationManager configuration)
    {

        services.AddTransient<IFeatureFlagService, FeatureFlagService>();
        services.AddTransient<IGrowthBookFeatureCache, FeatureCacheService>();
        services.AddTransient<IGrowthBookFeatureRepository, FeatureRepositoryService>();
        services.Configure<GrowthBookConfigurationOptions>(configuration.GetSection("GrowthBook"));

        return services;
    }
} 