using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.PreCacher.Services;

public class PreCacheService : BackgroundService
{
    private readonly IMongoService<CuratedFilter> _curatedFilter;
    private readonly IMongoService<CuratedList> _curatedList;
    private readonly IEntityCacheService _entityCacheService;
    private readonly ILogger<PreCacheService> _logger;

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="entityCacheService"></param>
    /// <param name="curatedList"></param>
    /// <param name="curatedFilter"></param>
    public PreCacheService(
        ILogger<PreCacheService> logger,
        IEntityCacheService entityCacheService,
        IMongoService<CuratedList> curatedList,
        IMongoService<CuratedFilter> curatedFilter
    )
    {
        _logger = logger;
        _entityCacheService = entityCacheService;
        _curatedList = curatedList;
        _curatedFilter = curatedFilter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                var curatedListList = await _curatedList.GetAllAsync();
                var curatedLists = curatedListList as CuratedList[] ?? curatedListList.ToArray();
                _logger.LogInformation("Pre caching GeInstrument with CuratedList total: {Count}", curatedLists.Length);
                foreach (var curatedList in curatedLists)
                    await _entityCacheService.PreCacheGeInstrument(curatedList, new TimeSpan(0, 8, 0, 0));

                var curatedFilterList = await _curatedFilter.GetAllAsync();
                var curatedFilters = curatedFilterList as CuratedFilter[] ?? curatedFilterList.ToArray();
                _logger.LogInformation("Pre caching GeInstrument with CuratedFilter total: {Count}",
                    curatedFilters.Length);
                foreach (var curatedFilter in curatedFilters)
                    await _entityCacheService.PreCacheGeInstrument(curatedFilter, new TimeSpan(0, 8, 0, 0));

                await Task.Delay(4 * 60 * 1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
    }
}