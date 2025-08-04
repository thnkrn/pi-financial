using GrowthBook;
using Pi.MarketData.Application.Interfaces;
using Pi.MarketData.Application.Services;

namespace Pi.MarketData.MigrationProxy.API.Startup;

public static class GrowthBookRegistrationExtensions
{
    public static IServiceCollection AddGrowthBookService(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddTransient<IFeatureFlagService, FeatureFlagService>();
        services.AddTransient<IGrowthBookFeatureCache, FeatureCacheService>();
        services.AddTransient<IGrowthBookFeatureRepository, FeatureRepositoryService>();
        services.Configure<GrowthBookConfigurationOptions>(configuration.GetSection("GrowthBook"));

        return services;
    }
}