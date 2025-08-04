using Pi.SetMarketData.DataProcessingService.Interface;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketData.DataProcessingService.Handlers;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
public class KafkaMessageHandlerMoreDependencies
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cacheService"></param>
    /// <param name="cacheServiceHelper"></param>
    /// <param name="morningStarFlagService"></param>
    /// <param name="scopeFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public KafkaMessageHandlerMoreDependencies
    (
        IRedisV2Publisher cacheService,
        ICacheServiceHelper cacheServiceHelper,
        IMongoService<MorningStarFlag> morningStarFlagService,
        IServiceScopeFactory scopeFactory
    )
    {
        CacheServiceHelper = cacheServiceHelper ?? throw new ArgumentNullException(nameof(cacheServiceHelper));
        CacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        MorningStarFlagService =
            morningStarFlagService ?? throw new ArgumentNullException(nameof(morningStarFlagService));
        ScopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }

    public ICacheServiceHelper CacheServiceHelper { get; set; }
    public IRedisV2Publisher CacheService { get; set; }
    public IMongoService<MorningStarFlag> MorningStarFlagService { get; set; }
    public IServiceScopeFactory ScopeFactory { get; set; }
}