using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.Common.HealthCheck;
using Pi.SetMarketDataWSS.Application.Interfaces;
using Pi.SetMarketDataWSS.Application.Interfaces.ItchHousekeeper;
using Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;
using Pi.SetMarketDataWSS.Application.Interfaces.OrderBookMapper;
using Pi.SetMarketDataWSS.Application.Interfaces.StreamingResponseBuilder;
using Pi.SetMarketDataWSS.Application.Services.ItchHousekeeper;
using Pi.SetMarketDataWSS.Application.Services.ItchMapper;
using Pi.SetMarketDataWSS.Application.Services.StreamingResponseBuilder;
using Pi.SetMarketDataWSS.DataSubscriber.BidOfferMapper;
using Pi.SetMarketDataWSS.DataSubscriber.Handlers;
using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Redis;
using Pi.SetMarketDataWSS.Infrastructure.Services.Kafka;
using Pi.SetMarketDataWSS.Infrastructure.Services.Redis;

namespace Pi.SetMarketDataWSS.DataSubscriber.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register IHealthCheckPublisher and HealthCheckPublisher
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

        // Register KafkaMessageHandler
        services.AddSingleton<KafkaMessageHandler>();

        services.AddSingleton<KafkaMessageHandlerDependencies>();

        // Register IRedisConnectionProvider and RedisConnectionProvider
        services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();

        // Register IRedisV2Publisher and RedisV2Publisher
        services.AddSingleton<IRedisV2Publisher, RedisV2Publisher>();

        // Register IBidOfferService and IBidOfferService
        services.AddSingleton<IBidOfferService, BidOfferService>();

        // Register IKafkaMessageHandler and KafkaMessageHandler
        services.AddSingleton<IKafkaMessageHandler<Message<string, string>>, KafkaMessageHandler>();

        // Register IKafkaSubscriber and KafkaSubscriber
        services.AddSingleton<IKafkaSubscriber>(provider =>
            new KafkaSubscriber<string, string>(
                configuration,
                configuration[ConfigurationKeys.KafkaTopic] ?? string.Empty,
                provider.GetRequiredService<IKafkaMessageHandler<Message<string, string>>>(),
                provider.GetRequiredService<ILogger<KafkaSubscriber<string, string>>>()));

        // Register IItchMapperService and ItchMapperService
        services.AddSingleton<IItchMapperService, ItchMapperService>();

        // Register IItchPriceInfoMapperService
        services.AddSingleton<IItchPriceInfoMapperService, ItchPriceInfoMapperService>();

        // Register IMarketStreamingResponseBuilder and MarketStreamingResponseBuilder
        services.AddSingleton<IMarketStreamingResponseBuilder, MarketStreamingResponseBuilder>();

        // Register IItchPublicTradeMapper and ItchPublicTradeMapperService
        services.AddSingleton<IItchPublicTradeMapper, ItchPublicTradeMapperService>();

        // Register IOrderBookMapper and ItchOrderBookMapperService
        services.AddSingleton<IOrderBookMapper, ItchOrderBookMapperService>();

        // Register IItchHousekeeperService and ItchHousekeeperService
        services.AddSingleton<IItchHousekeeperService, ItchHousekeeperService>();

        // Register IItchTickSizeTableEntryMapperService and ItchTickSizeTableEntryMapperService
        services.AddSingleton<IItchTickSizeTableEntryMapperService, ItchTickSizeTableEntryMapperService>();

        // Register ItchOpenInterest mapping service
        services.AddSingleton<IItchOpenInterestMapperService, ItchOpenInterestmapperService>();

        return services;
    }
}