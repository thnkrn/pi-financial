using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenSearch.Client;
using Pi.MarketData.Search.Application.Configs;
using Pi.MarketData.Search.Domain.Models;

namespace Pi.MarketData.Search.Application.Services;

public interface IInstrumentSearchService
{
    Task<SearchInstrumentResponse> SearchInstrumentsAsync(string keyword, string? instrumentType);
    Task<SearchInstrumentResponse> SearchInstrumentsAndEnhanceWithFavoritesAsync(string keyword, string? instrumentType, UserFavoriteResponse? userFavorites);
    Task<SearchInstrumentResponse> SearchInstrumentsAndEnhanceWithFavoritesAndStreamingDataAsync(string keyword, InstrumentType? instrumentType, UserFavoriteResponse? userFavorites, CancellationToken ct);
}

public class InstrumentSearchService : IInstrumentSearchService
{
    private readonly IOpenSearchClient _openSearchClient;
    private readonly ILogger<InstrumentSearchService> _logger;
    private readonly IStreamingDataCacheService _streamingDataCacheService;
    private readonly ILogoService _logoService;
    private readonly IFundMarketDataService _fundMarketDataService;
    private readonly InstrumentOrderOptions _options;

    public InstrumentSearchService(IOpenSearchClient openSearchClient, ILogger<InstrumentSearchService> logger,
        IStreamingDataCacheService streamingDataCacheService, ILogoService logoService,
        IFundMarketDataService fundMarketDataService, IOptions<InstrumentOrderOptions> options)
    {
        _openSearchClient = openSearchClient;
        _logger = logger;
        _streamingDataCacheService = streamingDataCacheService;
        _logoService = logoService;
        _fundMarketDataService = fundMarketDataService;
        _options = options.Value;
    }

    public async Task<SearchInstrumentResponse> SearchInstrumentsAsync(string keyword, string? instrumentType)
    {
        var searchRequest = CreateSearchRequest(keyword, instrumentType);
        var searchResponse = await _openSearchClient.SearchAsync<SearchInstrumentDocument>(searchRequest);
        _logger.LogInformation("Search valid: {Valid}", searchResponse.IsValid);
        _logger.LogInformation("Search response: {Response}", searchResponse.Hits.Count);
        return TransformToSearchResult(searchResponse, null, null);
    }

    public async Task<SearchInstrumentResponse> SearchInstrumentsAndEnhanceWithFavoritesAsync(string keyword, string? instrumentType, UserFavoriteResponse? userFavorites)
    {
        var searchRequest = CreateSearchRequest(keyword, instrumentType);
        var searchResponse = await _openSearchClient.SearchAsync<SearchInstrumentDocument>(searchRequest);
        _logger.LogInformation("Search valid: {Valid}", searchResponse.IsValid);
        _logger.LogInformation("Search response: {Response}", searchResponse.Hits.Count);
        return TransformToSearchResult(searchResponse, userFavorites, null);
    }

    public async Task<SearchInstrumentResponse> SearchInstrumentsAndEnhanceWithFavoritesAndStreamingDataAsync(
        string keyword, InstrumentType? instrumentType, UserFavoriteResponse? userFavorites, CancellationToken ct)
    {
        var tOtherInstruments = Task.FromResult(new SearchInstrumentResponse { Data = [] });
        var tMutualFunds = Task.FromResult(new SearchInstrumentResponse { Data = [] });

        switch (instrumentType)
        {
            case InstrumentType.Fund:
                tMutualFunds = GetFundSearch(keyword, userFavorites, ct);
                break;
            case null:
                tOtherInstruments = GetOtherInstrumentSearch(keyword, instrumentType, userFavorites, ct);
                tMutualFunds = GetFundSearch(keyword, userFavorites, ct);
                break;
            default:
                tOtherInstruments = GetOtherInstrumentSearch(keyword, instrumentType, userFavorites, ct);
                break;
        }

        await Task.WhenAll(tOtherInstruments, tMutualFunds);

        var mfResult = await tMutualFunds;
        var otherInstruments = await tOtherInstruments;

        var combinedResult = mfResult.Data.Concat(otherInstruments.Data).OrderBy(x => x.Order).ToList();

        var response = new SearchInstrumentResponse { Data = combinedResult };

        return response;
    }

    private async Task<SearchInstrumentResponse> GetOtherInstrumentSearch(string keyword, InstrumentType? instrumentType, UserFavoriteResponse? userFavorites, CancellationToken ct)
    {
        var searchRequest = CreateSearchRequest(keyword, instrumentType.ToString());
        var searchResponse = await _openSearchClient.SearchAsync<SearchInstrumentDocument>(searchRequest, ct);
        _logger.LogInformation("Search valid: {Valid}", searchResponse.IsValid);
        _logger.LogInformation("Search response: {Response}", searchResponse.Hits.Count);
        // Get streaming data for all instruments
        var streamingDataTasks = searchResponse.Hits
            .Where(hit => hit.Source.Symbol != null)
            .Select(async hit =>
            {
                var findingKey = hit.Source.Symbol;
                if ((hit.Source.Type == "Equity" || hit.Source.Type == "Derivative") && !string.IsNullOrEmpty(hit.Source.OrderBookId))
                {
                    findingKey = hit.Source.OrderBookId;
                }
                return new
                {
                    // Add more uniqueness to the key by including Type
                    Key = $"{hit.Source.Venue}::{hit.Source.Symbol}::{hit.Source.Type}",
                    StreamingData = await _streamingDataCacheService.GetStreamingDataAsync(findingKey!)
                };
            });

        var streamingDataResults = await Task.WhenAll(streamingDataTasks);

        // Use GroupBy and take the first item in case of duplicates
        var streamingDataMap = streamingDataResults
            .GroupBy(x => x.Key)
            .ToDictionary(
                g => g.Key,
                g => g.First().StreamingData
            );

        var result = TransformToSearchResult(searchResponse, userFavorites, streamingDataMap);

        return result;
    }

    private async Task<SearchInstrumentResponse> GetFundSearch(string keyword, UserFavoriteResponse? userFavorites, CancellationToken ct)
    {
        var orderConfig = _options.Sequence.ToList();
        var fundResult = await _fundMarketDataService.SearchFunds(keyword, ct);

        if (!fundResult.Any())
        {
            return new SearchInstrumentResponse { Data = [] };
        }

        var instruments = fundResult
            .Select(x => MapToSearchInstrumentItemResponse(userFavorites, x))
            .ToList();

        var fundSearchResult = new SearchInstrumentGroupResponse
        {
            Type = InstrumentType.Fund.ToString(),
            Category = InstrumentCategory.Funds.ToString(),
            Order = orderConfig.IndexOf(InstrumentCategory.Funds) + 1,
            Instruments = instruments
        };

        return new SearchInstrumentResponse { Data = [fundSearchResult] };
    }

    private static SearchRequest CreateSearchRequest(string keyword, string? instrumentType)
    {
        var shouldQueries = new List<QueryContainer>
        {
            new TermQuery
            {
                Field = "symbol",
                Value = keyword,
                Boost = 300,
                CaseInsensitive = true
            },
            new TermQuery
            {
                Field = "friendlyName.keyword",
                Value = keyword,
                Boost = 250,
                CaseInsensitive = true
            },
            new PrefixQuery
            {
                Field = "symbol",
                Value = keyword,
                Boost = 50,
                CaseInsensitive = true
            },
            new TermQuery
            {
                Field = "customIndex",
                Value = keyword,
                Boost = 40,
                CaseInsensitive = true
            },
            new MatchQuery
            {
                Field = "symbol.ngram",
                Query = keyword,
                Boost = 20,
            },
            new MatchQuery
            {
                Field = "name.ngram",
                Query = keyword,
                Boost = 4
            },
            new MatchQuery
            {
                Field = "friendlyName.ngram",
                Query = keyword,
                Boost = 2
            }
        };

        var boolQuery = new BoolQuery { };

        var must = new List<QueryContainer>
        {
            new TermQuery
            {
                Field = "status",
                Value = "enabled"
            }
        };

        if (!string.IsNullOrWhiteSpace(instrumentType) && instrumentType != "all")
        {
            must.Add(new TermQuery
            {
                Field = "type",
                Value = instrumentType
            });
        }

        boolQuery.Must = must;
        boolQuery.Should = shouldQueries;
        boolQuery.MinimumShouldMatch = 1;

        return new SearchRequest<SearchInstrumentDocument>
        {
            Query = boolQuery,
            Size = 100,
            Sort = new[] { new FieldSort { Field = "_score" } }
        };
    }

    private SearchInstrumentResponse TransformToSearchResult(ISearchResponse<SearchInstrumentDocument> searchResponse, UserFavoriteResponse? userFavorites, Dictionary<string, StreamingDataResponse>? streamingDataMap)
    {
        // NOTE: Deduplicate in application layer since we don't want to effect the indexing side due to existing data
        var deduplicatedHits = searchResponse.Hits
            .Where(h =>
                // NOTE: Exclude if deprecated
                h.Source?.Deprecated != true &&
                // NOTE: Remove warants with -F and -O out
                !(h.Source?.SecurityType is "CS" or "CSF" or "W" &&
                  (h.Source?.Symbol?.IndexOf("-f", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   h.Source?.Symbol?.IndexOf("-o", StringComparison.OrdinalIgnoreCase) >= 0)))
            .GroupBy(h => new
            {
                Symbol = h.Source?.Symbol,
                Venue = h.Source?.Venue,
                Type = h.Source?.Type
            })
            .Select(g =>
            {
                // Exclude only symbol that have disabled status, otherwise we show them up
                var hit = g.FirstOrDefault(x => x.Source?.Status?.ToLowerInvariant() != "disabled");
                return hit ?? g.First();
            })
            .ToList();

        var grouped = deduplicatedHits
            .GroupBy(h => new { Type = h.Source?.Type, Category = h.Source?.Category })
            .Select(g =>
            {
                var type = g.First().Source.Type;
                var category = g.First().Source.Category ?? "";
                return new SearchInstrumentGroupResponse
                {
                    Type = type,
                    Category = category,
                    // this will be use when sorted outside this function after merge with mutual fund
                    Order = 0 - ((int)(g.Max(h => h.Score) ?? 0)),
                    Instruments = g.Select(h =>
                    {
                        var streamingData = streamingDataMap?.GetValueOrDefault($"{h.Source.Venue}::{h.Source.Symbol}");
                        var logoSymbol = h.Source?.Symbol;
                        var logoCategory = h.Source?.Category;
                        if (!string.IsNullOrEmpty(h.Source?.UnderlyingSymbol))
                        {
                            logoSymbol = h.Source?.UnderlyingSymbol;
                            logoCategory = null;
                        }

                        var logoUrl = h.Source?.Venue != null && logoSymbol != null
                            ? _logoService.GetLogoUrl(h.Source.Venue, logoSymbol, logoCategory ?? "")
                            : "";
                        return new SearchInstrumentItemResponse
                        {
                            Symbol = h.Source?.Symbol ?? "",
                            Name = h.Source?.Name ?? "",
                            FriendlyName = h.Source?.FriendlyName ?? "",
                            Type = h.Source?.Type ?? "",
                            Category = h.Source?.Category ?? "",
                            IsFavorite = userFavorites?.Data?.Any(f =>
                            f.Symbol == h.Source?.Symbol &&
                            f.Venue == h.Source.Venue) ?? false,
                            Unit = h.Source?.Currency ?? "",
                            Logo = logoUrl,
                            Price = streamingData?.Price ?? "0.00",
                            PriceChange = streamingData?.PriceChanged ?? "0.00",
                            PriceChangeRatio = streamingData?.PriceChangedRate ?? "0.00",
                            OrderBookId = h.Source?.OrderBookId ?? "",
                            Venue = h.Source?.Venue ?? "",
                            EsScore = h.Score?.ToString() ?? "0"
                        };
                    }).ToList()
                };
            }).ToList();

        return new SearchInstrumentResponse { Data = grouped };
    }

    private static SearchInstrumentItemResponse MapToSearchInstrumentItemResponse(UserFavoriteResponse? userFavorites, InstrumentSummary instrument)
    {
        var response = new SearchInstrumentItemResponse
        {
            Venue = instrument.Venue,
            Symbol = instrument.Symbol,
            Name = instrument.FriendlyName,
            FriendlyName = instrument.FriendlyName,
            Logo = instrument.Logo,
            IsFavorite = userFavorites?.Data.Any(f => f.Symbol == instrument.Symbol && f.Venue == instrument.Venue) ?? false,
            Unit = instrument.Currency,
            Type = instrument.Type.ToString(),
            Category = instrument.Category.ToString(),
            Price = instrument.Price.ToString(CultureInfo.InvariantCulture),
            PriceChangeRatio = instrument.PriceChangeRatio.ToString(CultureInfo.InvariantCulture),
            Nav = string.Empty,
            NavChange = string.Empty,
            NavChangePercentage = string.Empty
        };

        return response;
    }

}
