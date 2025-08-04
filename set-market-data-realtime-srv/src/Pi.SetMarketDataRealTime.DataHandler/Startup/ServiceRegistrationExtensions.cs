using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.Common.HealthCheck;
using Pi.SetMarketDataRealTime.Application.Interfaces.Holiday;
using Pi.SetMarketDataRealTime.Application.Interfaces.ItchParser;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Interfaces.MessageValidator;
using Pi.SetMarketDataRealTime.Application.Interfaces.WriteBinlogData;
using Pi.SetMarketDataRealTime.Application.Queries.Holiday;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.MessageValidator;
using Pi.SetMarketDataRealTime.Application.Services.WriteBinLogData;
using Pi.SetMarketDataRealTime.DataHandler.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.DataHandler.Services.SoupBinTcp;
using Pi.SetMarketDataRealTime.Domain.AggregatesModels.HolidayAggregate;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.AmazonS3;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Repositories;
using Pi.SetMarketDataRealTime.Infrastructure.Services.AmazonS3;
using Pi.SetMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;
using Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp;

namespace Pi.SetMarketDataRealTime.DataHandler.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register IHealthCheckPublisher and HealthCheckPublisher
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

        // Register IItchParserService and ItchParserService
        services.AddSingleton<IItchParserService, ItchParserService>();

        // Register IClient and Client
        services.AddSingleton<IClient, Client>();

        // Register IHolidayApiRepository and HolidayApiRepository
        services.AddSingleton<IHolidayApiRepository, HolidayApiRepository>();

        // Register IDateTimeProvider and SystemDateTimeProvider
        services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();

        // Register IHolidayQuery and HolidayQuery
        services.AddScoped<IHolidayApiQuery, HolidayApiQuery>();

        // Register IClientFactory and ClientFactory
        services.AddSingleton<IClientFactory, ClientFactory>();

        // Register ClientSubscriptionDependencies
        services.AddSingleton<ClientSubscriptionDependencies>();

        // Register ClientSubscriptionServiceAutoRestart
        services.AddSingleton<ClientSubscriptionServiceAutoRestart>();

        // Register IDisconnectionHandler
        services.AddSingleton<IDisconnectionHandler>(sp =>
            sp.GetRequiredService<ClientSubscriptionServiceAutoRestart>());

        // Register IDisconnectionHandlerFactory and DisconnectionHandlerFactory
        services.AddSingleton<IDisconnectionHandlerFactory, DisconnectionHandlerFactory>();

        // Register ClientListenerDependencies
        services.AddSingleton<ClientListenerDependencies>();

        // Register IClientListener and ClientListener
        services.AddSingleton<IClientListener, ClientListener>();

        // High-Performance Kafka Producer
        services.AddSingleton<IKafkaProducerOptimizedV2Service, KafkaProducerOptimizedV2Service>();

        // Register base StockDataOptimizedPublisher as a separate service
        services.AddSingleton<IStockDataOptimizedV2Publisher, StockDataOptimizedV2Publisher>();

        // Register IWriteBinLogsData and WriteBinLogsData
        services.AddSingleton<IWriteBinLogsData, WriteBinLogsData>();

        // Register IMemoryCacheWrapper and MemoryCacheWrapper
        services.AddSingleton<IMemoryCacheWrapper, MemoryCacheWrapper>();

        // Register IMemoryCacheWrapper and MemoryCacheWrapper
        services.AddSingleton<IMemoryCacheHelper, MemoryCacheHelper>();

        // Register ItchMessageMetadataHandler
        services.AddSingleton<ItchMessageMetadataHandler>();

        // Register IMessageValidator and MessageValidator
        services.AddSingleton<IMessageValidator, MessageValidator>();

        // Register IAmazonS3Service and AmazonS3Service
        services.AddSingleton<IAmazonS3Service, AmazonS3Service>();

        // Register RealTimeStockMessageLogger
        services.AddSingleton<RealTimeStockMessageLogger>();

        return services;
    }
}