using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Pi.Common.HealthCheck;
using Pi.GlobalMarketDataRealTime.DataHandler.Exceptions;
using Pi.GlobalMarketDataRealTime.DataHandler.Helpers;
using Pi.GlobalMarketDataRealTime.DataHandler.Interfaces;
using Pi.GlobalMarketDataRealTime.DataHandler.Models.FixModel;
using Pi.GlobalMarketDataRealTime.DataHandler.Services.FixService;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Fix;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Mongo;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Redis;
using HealthCheckService = Pi.GlobalMarketDataRealTime.DataHandler.Services.HealthService.HealthCheckService;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration,
        ILogger logger)
    {
        RegisterCommonServices(services, configuration);
        RegisterFixServices(services, configuration, logger);
        RegisterHealthChecks(services);

        return services;
    }

    private static void RegisterCommonServices(IServiceCollection services, IConfiguration configuration)
    {
        // High-Performance Kafka Producer
        services.AddSingleton<IKafkaProducerOptimizedV2Service, KafkaProducerOptimizedV2Service>();

        // Register base StockDataOptimizedPublisher as a separate service
        services.AddSingleton<IStockDataOptimizedV2Publisher, StockDataOptimizedV2Publisher>();

        // Rest of the registrations
        services.AddSingleton<IKafkaPublisher, KafkaPublisher>();

        // Register IRedisConnectionProvider and RedisConnectionProvider
        services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();
        
        // Register IRedisV2Publisher and RedisV2Publisher
        services.AddSingleton<IRedisV2Publisher, RedisV2Publisher>();

        services.AddSingleton<IDataRecoveryService, DataRecoveryService>();
        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        services.AddSingleton<IVelexaHttpApiJwtTokenGenerator, VelexaHttpApiJwtTokenGenerator>(provider =>
            new VelexaHttpApiJwtTokenGenerator(
                configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiJwtSecret) ?? string.Empty,
                configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiJwtClientId) ?? string.Empty,
                configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiJwtAppId) ?? string.Empty,
                provider.GetRequiredService<ILogger<VelexaHttpApiJwtTokenGenerator>>()
            )
        );
    }

    private static void RegisterFixServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        var fixConfig = new FIX();
        configuration.GetSection(ConfigurationKeys.FixConfig).Bind(fixConfig);

        var fixConfigFiles = configuration.GetValue(ConfigurationKeys.FixConfigFiles, string.Empty);
        if (!string.IsNullOrWhiteSpace(fixConfigFiles))
        {
            var files = JsonConvert.DeserializeObject<List<FixConfig>>(fixConfigFiles);
            if (files != null)
                fixConfig.CONFIG_FILES = [..files];
        }

        if (fixConfig.CONFIG_FILES == null)
            throw new SubscriptionServiceException(nameof(fixConfig.CONFIG_FILES));

        bool retry;
        do
        {
            retry = false;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var configFile in fixConfig.CONFIG_FILES.Select(cf => cf.ConfigFile).Where(cf => cf != null))
                if (configFile != null && TryRegisterFixServices(services, configFile, logger))
                {
                    retry = true;
                    break;
                }
        } while (!retry);
    }

    private static bool TryRegisterFixServices(IServiceCollection services,
        string configFile, ILogger logger)
    {
        var result = false;
        try
        {
            RegisterFixListener(services, configFile);
            RegisterFixClientManager(services, configFile);

            if (TestFixConnection(services))
            {
                logger.LogDebug("The config file ({ConfigFile}) can connect to FIX.", configFile);
                result = true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {Message}", ex.Message);
        }
        finally
        {
            if (!result)
            {
                logger.LogWarning("Unable to connect using config file ({ConfigFile}). Try the next file again.",
                    configFile);
                Task.Delay(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
                RemoveFailureServices(services);
            }
        }

        return result;
    }

    private static void RegisterFixListener(IServiceCollection services, string configFile)
    {
        services.AddSingleton<ClientSubscriptionV2Service>();
        services.AddSingleton<MarketScheduleService>();

        // Then register it as a hosted service
        services.AddHostedService(provider => provider.GetRequiredService<ClientSubscriptionV2Service>());

        services.AddHostedService(provider => provider.GetRequiredService<MarketScheduleService>());

        // Register it as ILogoutNotificationHandler
        services.AddSingleton<ILogoutNotificationHandler>(provider =>
            provider.GetRequiredService<ClientSubscriptionV2Service>());

        // Then register other services
        services.AddSingleton<IFixListener, EnhancedFixListener>(provider => new EnhancedFixListener(
            configFile,
            provider.GetRequiredService<IDataRecoveryService>(),
            provider.GetRequiredService<IStockDataOptimizedV2Publisher>(),
            provider.GetRequiredService<ILogger<EnhancedFixListener>>(),
            provider
        ));
    }

    private static void RegisterFixClientManager(IServiceCollection services, string configFile)
    {
        // Register the client factory as a singleton
        services.AddSingleton<IFixClientFactory>(serviceProvider =>
            new FixClientFactory(serviceProvider, configFile));

        // Register the IClient as a factory that uses our FixClientFactory
        services.AddTransient<IClient>(serviceProvider =>
            serviceProvider.GetRequiredService<IFixClientFactory>().GetClient());
    }

    private static bool TestFixConnection(IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();
        var client = sp.GetService<IClient>();
        client?.StartAsync().GetAwaiter();

        var listener = sp.GetService<IFixListener>();
        Task.Delay(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();

        var isSessionLogon = listener != null && listener.CheckSession();

        if (client is { State: ClientState.Connected })
        {
            client.ShutdownAsync().GetAwaiter();
            client.Dispose();
        }

        return isSessionLogon;
    }

    private static void RemoveFailureServices(IServiceCollection services)
    {
        var client = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IClient));
        var listener = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IFixListener));

        if (client != null)
            services.Remove(client);
        if (listener != null)
            services.Remove(listener);
    }

    private static void RegisterHealthChecks(IServiceCollection services)
    {
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

        services.AddHealthChecks()
            .AddCheck("Liveness", () => HealthCheckResult.Healthy(), ["liveness"])
            .AddCheck("Readiness", () => HealthCheckResult.Healthy(), ["readiness"]);

        services.AddHostedService<HealthCheckService>();
    }
}