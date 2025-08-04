using OpenSearch.Client;

using Pi.MarketData.Domain.Models;

namespace Pi.MarketData.SearchAPI.Services;

public interface IInstrumentSearchService
{
    Task<SearchInstrumentResponse> SearchInstrumentsAsync(string keyword, string? instrumentType);
    Task<SearchInstrumentResponse> SearchInstrumentsAndEnhanceWithFavoritesAsync(string keyword, string? instrumentType, UserFavoriteResponse? userFavorites);
    Task<SearchInstrumentResponse> SearchInstrumentsAndEnhanceWithFavoritesAndStreamingDataAsync(string keyword, string? instrumentType, UserFavoriteResponse? userFavorites);
}

public class InstrumentSearchService : IInstrumentSearchService
{
    private readonly IOpenSearchClient _openSearchClient;
    private readonly ILogger<InstrumentSearchService> _logger;
    private readonly IStreamingDataCacheService _streamingDataCacheService;
    private readonly ILogoService _logoService;
    public InstrumentSearchService(IOpenSearchClient openSearchClient, ILogger<InstrumentSearchService> logger, IStreamingDataCacheService streamingDataCacheService, ILogoService logoService)
    {
        _openSearchClient = openSearchClient;
        _logger = logger;
        _streamingDataCacheService = streamingDataCacheService;
        _logoService = logoService;
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

    public async Task<SearchInstrumentResponse> SearchInstrumentsAndEnhanceWithFavoritesAndStreamingDataAsync(string keyword, string? instrumentType, UserFavoriteResponse? userFavorites)
    {
        var searchRequest = CreateSearchRequest(keyword, instrumentType);
        var searchResponse = await _openSearchClient.SearchAsync<SearchInstrumentDocument>(searchRequest);
        _logger.LogInformation("Search valid: {Valid}", searchResponse.IsValid);
        _logger.LogInformation("Search response: {Response}", searchResponse.Hits.Count);

        // Get streaming data for all instruments
        var streamingDataTasks = searchResponse.Hits.Select(async h =>
        {
            var findingKey = h.Source.Symbol;
            if ((h.Source.Type == "Equity" || h.Source.Type == "Derivative") && !string.IsNullOrEmpty(h.Source.OrderBookId))
            {
                findingKey = h.Source.OrderBookId;
            }
            return new
            {
                Symbol = h.Source.Symbol,
                Venue = h.Source.Venue,
                StreamingData = await _streamingDataCacheService.GetStreamingDataAsync(findingKey)
            };
        });

        var streamingDataResults = await Task.WhenAll(streamingDataTasks);
        var streamingDataMap = streamingDataResults.ToDictionary(x => $"{x.Venue}::{x.Symbol}", x => x.StreamingData);

        return TransformToSearchResult(searchResponse, userFavorites, streamingDataMap);
    }

    private static SearchRequest CreateSearchRequest(string keyword, string? instrumentType)
    {
        var shouldQueries = new List<QueryContainer>
        {
            new PrefixQuery
            {
                Field = "symbol",
                Value = keyword,
                Boost = 10
            },
            new MatchQuery
            {
                Field = "symbol.ngram",
                Query = keyword,
                Boost = 8
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

        if (!string.IsNullOrEmpty(instrumentType) && instrumentType != "all")
        {
            boolQuery.Must = new QueryContainer[]
            {
                new TermQuery
                {
                    Field = "type",
                    Value = instrumentType
                }
            };
        }
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
        var grouped = searchResponse.Hits
            .GroupBy(h => h.Source.Type)
            .Select(g =>
            {
                return new SearchInstrumentGroupResponse
                {
                    Type = g.Key,
                    Category = g.First().Source.Category,
                    Order = 0,
                    Instruments = g.Select(h =>
                    {
                        var streamingData = streamingDataMap?.GetValueOrDefault($"{h.Source.Venue}::{h.Source.Symbol}");
                        return new SearchInstrumentItemResponse
                        {
                            Symbol = h.Source.Symbol,
                            Name = h.Source.Name,
                            FriendlyName = h.Source.FriendlyName,
                            Type = h.Source.Type,
                            Category = h.Source.Category,
                            IsFavorite = userFavorites?.Data?.Any(f =>
                            f.Symbol == h.Source.Symbol &&
                            f.Venue == h.Source.Venue) ?? false,
                            Unit = h.Source.Currency,
                            Logo = _logoService.GetLogoUrl(h.Source.Venue, h.Source.Symbol),
                            Price = streamingData?.Price ?? "0.00",
                            PriceChange = streamingData?.PriceChanged ?? "0.00",
                            PriceChangeRatio = streamingData?.PriceChangedRate ?? "0.00",
                            OrderBookId = h.Source.OrderBookId,
                            Venue = h.Source.Venue
                        };
                    }).ToList()
                };
            })
        .ToList();
        return new SearchInstrumentResponse { Data = grouped };
    }
}