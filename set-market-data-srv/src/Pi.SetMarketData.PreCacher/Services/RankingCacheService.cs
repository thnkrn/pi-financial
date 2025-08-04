using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Application.Helper;
using Pi.SetMarketData.Application.Interfaces.Holiday;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.SetMarketData.PreCacher.Services;

public class RankingCacheService : BackgroundService
{
    private const int MaxBatchSize = 100;
    private readonly IMongoService<CuratedFilter> _curatedFilterService;
    private readonly IMongoService<CuratedList> _curatedListService;
    private readonly IEntityCacheService _entityCacheService;
    private readonly IHolidayApiQuery _holidayApiQuery;
    private readonly IMongoService<Instrument> _instrumentService;
    private readonly ILogger<RankingCacheService> _logger;

    private readonly Stopwatch _stopwatch = new();
    private readonly ITimescaleService<RealtimeMarketData> _timescaleService;

    public RankingCacheService(
        ILogger<RankingCacheService> logger,
        IEntityCacheService entityCacheService,
        ITimescaleService<RealtimeMarketData> timescaleService,
        IMongoService<Instrument> instrumentService,
        IMongoService<CuratedList> curatedListService,
        IMongoService<CuratedFilter> curatedFilterService,
        IHolidayApiQuery holidayApiQuery
    )
    {
        _logger = logger;
        _entityCacheService = entityCacheService;
        _timescaleService = timescaleService;
        _instrumentService = instrumentService;
        _curatedListService = curatedListService;
        _curatedListService = curatedListService;
        _curatedFilterService = curatedFilterService;
        _holidayApiQuery = holidayApiQuery;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RankingCacheService starting");

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                _logger.LogInformation("Starting ranking cache cycle");

                var instruments = await _instrumentService.GetAllAsync();
                var groupedSymbols = instruments
                    .Where(instrument =>
                        !string.IsNullOrEmpty(instrument.Symbol)
                        && !string.IsNullOrEmpty(instrument.Venue)
                    )
                    .GroupBy(instrument => instrument.Venue)
                    .Select(group => new
                    {
                        Venue = group.Key,
                        Symbols = group.Select(g => g.Symbol).ToArray()
                    })
                    .ToList();

                var rankingItems = new List<RankingItem>();
                foreach (var group in groupedSymbols)
                {
                    var marketStartTime = MarketTime.StartTime.TryGetValue(group.Venue, out var venue) ? venue : null;

                    if (string.IsNullOrEmpty(marketStartTime))
                        continue;

                    var rankingStartDate =
                        await RankingHelper.CalculateRankingStartDate(DateTime.UtcNow, marketStartTime,
                            _holidayApiQuery);
                    var rankingItem = await GetRankingItemsAsync(group.Symbols, group.Venue, rankingStartDate);
                    rankingItems.AddRange(rankingItem);
                }

                if (rankingItems.Count > 0)
                {
                    LogTopRankingItem(rankingItems);
                    await PreCacheAsync(rankingItems);
                    _logger.LogInformation(
                        "Retrieved ranking-items {Count}; Completed the get-ranking-items and pre-cache operations!",
                        rankingItems.Count);
                }
                else
                {
                    _logger.LogWarning("No ranking item found");
                }
                await Task.Delay(15 * 60 * 1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

        _logger.LogInformation("RankingCacheService stopped");
    }

    private async Task PreCacheAsync(List<RankingItem> rankingItems)
    {
        try
        {
            _stopwatch.Restart();
            await _entityCacheService.UpdateRankingItem(rankingItems);

            var curatedListList = await _curatedListService.GetAllAsync();
            foreach (var curatedList in curatedListList)
                await _entityCacheService.PreCacheSortedInstrument(curatedList, new TimeSpan(0, 8, 0, 0));

            var curatedFilterList = await _curatedFilterService.GetAllAsync();
            foreach (var curatedFilter in curatedFilterList)
                await _entityCacheService.PreCacheSortedInstrument(curatedFilter, new TimeSpan(0, 8, 0, 0));
        }
        finally
        {
            _stopwatch.Stop();
            _logger.LogInformation("Pre-cache operation completed in {ElapsedMilliseconds}ms",
                _stopwatch.ElapsedMilliseconds);
        }
    }

    private async Task<List<RankingItem>> GetRankingItemsAsync(string[] symbols, string venue,
        DateTime rankingStartDate)
    {
        try
        {
            _stopwatch.Restart();

            var rankingItems = new List<RankingItem>();
            var symbolChunks = symbols
                .Select((symbol, index) => new { symbol, index })
                .GroupBy(x => x.index / MaxBatchSize)
                .Select(g => g.Select(x => x.symbol).ToArray())
                .ToList();

            foreach (var chunk in symbolChunks)
            {
                var result = await _timescaleService.GetRankingItem(chunk, venue, chunk.Length, rankingStartDate);
                rankingItems.AddRange(result);
            }

            return rankingItems;
        }
        finally
        {
            _stopwatch.Stop();
            _logger.LogInformation("Get-ranking-items operation completed in {ElapsedMilliseconds}ms",
                _stopwatch.ElapsedMilliseconds);
        }
    }

    private void LogTopRankingItem(List<RankingItem> rankingItems)
    {
        var topRankingItem = rankingItems.OrderByDescending(x => x.Amount).Take(10).ToList();
        _logger.LogInformation("Top 10 ranking items");
        for (int i = 0; i < topRankingItem.Count; i++)
        {
            var item = topRankingItem[i];
            _logger.LogInformation("Rank: {index}, Venue: {Venue}, Symbol: {Symbol}, Amount: {Amount}", i + 1, item.Venue, item.Symbol, item.Amount);
        }
    }
}