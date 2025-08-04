using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.PreCacher.Services;

public class PreCacheService : BackgroundService
{
    private readonly ILogger<PreCacheService> _logger;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceProvider"></param>
    public PreCacheService(
        ILogger<PreCacheService> logger,
        IServiceProvider serviceProvider
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var entityCacheService = scope.ServiceProvider.GetRequiredService<IEntityCacheService>();
                    var curatedList = scope.ServiceProvider.GetRequiredService<IMongoService<CuratedList>>();
                    var curatedFilter = scope.ServiceProvider.GetRequiredService<IMongoService<CuratedFilter>>();

                    var curatedListList = await curatedList.GetAllAsync();
                    var curatedLists = curatedListList as CuratedList[] ?? curatedListList.ToArray();
                    _logger.LogInformation("Pre caching Instrument with CuratedList total: {Count}",
                        curatedLists.Length);
                    foreach (var list in curatedLists)
                        await entityCacheService.PreCacheInstrument(list, new TimeSpan(0, 8, 0, 0));

                    var curatedFilterList = await curatedFilter.GetAllAsync();
                    var curatedFilters = curatedFilterList as CuratedFilter[] ?? curatedFilterList.ToArray();
                    _logger.LogInformation("Pre caching Instrument with CuratedFilter total: {Count}",
                        curatedFilters.Length);
                    foreach (var filter in curatedFilters)
                        await entityCacheService.PreCacheInstrument(filter, new TimeSpan(0, 8, 0, 0));
                }

                await Task.Delay(4 * 60 * 1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
    }
}