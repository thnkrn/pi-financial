using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.GlobalMarketData.PreCacher.Services;

public class RankingCacheService : BackgroundService
{
    private const int MaxBatchSize = 100;
    private readonly IMongoService<CuratedFilter> _curatedFilterService;
    private readonly IMongoService<CuratedList> _curatedListService;
    private readonly IEntityCacheService _entityCacheService;
    private readonly IMongoService<GeInstrument> _geInstrumentService;
    private readonly ILogger<RankingCacheService> _logger;
    private readonly Stopwatch _stopwatch = new();
    private readonly ITimescaleService<RealtimeMarketData> _timescaleService;

    public RankingCacheService
    (
        ILogger<RankingCacheService> logger,
        IEntityCacheService entityCacheService,
        ITimescaleService<RealtimeMarketData> timescaleService,
        IMongoService<GeInstrument> geInstrumentService,
        IMongoService<CuratedList> curatedListService,
        IMongoService<CuratedFilter> curatedFilterService
    )
    {
        _logger = logger;
        _entityCacheService = entityCacheService;
        _timescaleService = timescaleService;
        _geInstrumentService = geInstrumentService;
        _curatedListService = curatedListService;
        _curatedListService = curatedListService;
        _curatedFilterService = curatedFilterService;
    }

    [SuppressMessage("SonarQube", "S3776")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RankingCacheService starting");

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                _logger.LogInformation("Starting ranking cache cycle");

                await _entityCacheService.UpdateWhiteList();
                var geGeInstrument = await _geInstrumentService.GetAllAsync();
                var groupedSymbols = geGeInstrument
                    .Where(instrument =>
                        !string.IsNullOrEmpty(instrument.Symbol)
                        && !string.IsNullOrEmpty(instrument.Venue)
                    )
                    .GroupBy(instrument => (instrument.Venue, instrument.Exchange))
                    .Select(group => new
                    {
                        group.Key.Venue,
                        group.Key.Exchange,
                        Symbols = group.Select(g => g.Symbol).ToArray()
                    })
                    .ToList();

                var rankingItems = new List<RankingItem>();
                foreach (var group in groupedSymbols)
                    try
                    {
                        var marketSchedules = await _entityCacheService.GetMarketSchedule(
                            string.Empty,
                            group.Exchange ?? string.Empty,
                            "MainSession",
                            DateTime.UtcNow.Date
                        );

                        var startTime = marketSchedules?.UTCStartTime;
                        if (!startTime.HasValue)
                        {
                            _logger.LogWarning("Market schedule found but UTCStartTime is null for venue: {Venue}",
                                group.Venue);
                            continue;
                        }

                        var rankingItem = await GetRankingItemsAsync(
                            group.Symbols,
                            group.Venue ?? string.Empty,
                            startTime.Value
                        );
                        rankingItems.AddRange(rankingItem);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error fetching market schedules for venue: {Venue}", group.Venue);
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
                    _logger.LogWarning("No ranking items found");
                }

                await Task.Delay(15 * 60 * 1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
    }

    private async Task PreCacheAsync(List<RankingItem> rankingItems)
    {
        try
        {
            _stopwatch.Restart();
            await _entityCacheService.UpdateRankingItem(rankingItems);

            var curatedListList = await _curatedListService.GetAllAsync();
            foreach (var curatedList in curatedListList)
                await _entityCacheService.PreCacheSortedGeInstrument(curatedList, new TimeSpan(0, 8, 0, 0));

            var curatedFilterList = await _curatedFilterService.GetAllAsync();
            foreach (var curatedFilter in curatedFilterList)
                await _entityCacheService.PreCacheSortedGeInstrument(curatedFilter, new TimeSpan(0, 8, 0, 0));
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

        for (var i = 0; i < topRankingItem.Count; i++)
        {
            var item = topRankingItem[i];
            _logger.LogInformation("Rank: {Index}, Venue: {Venue}, Symbol: {Symbol}, Amount: {Amount}", i + 1, item.Venue, item.Symbol, item.Amount);
        }
    }
}