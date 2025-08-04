using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.MarketData.Search.Application.Configs;
using Pi.MarketData.Search.Domain.ConstantConfigurations;
using Pi.MarketData.Search.Domain.Models;
using Pi.MarketData.Search.Domain.Models.Responses;

namespace Pi.MarketData.Search.Application.Services;

public interface IUserFavoriteAndPositionService
{
    Task<UserFavoriteResponse> GetPiWatchListAsync(string sessionId, bool useMockWatchList = false);
    Task<UserInstrumentFavoriteResponse> GetUserFavoritesAsyncAndEnhanceWithStreamingData(string userId, bool useMockWatchList = false, CancellationToken ct = default);
    Task<UserInstrumentPositionResponse> GetUserPositionsAsyncAndEnhanceWithStreamingData(string userId);
    List<string> MapCacheKeys(UserInstrumentFavoriteResponse response);
    UserInstrumentFavoriteResponse MapPrice(UserInstrumentFavoriteResponse response, IDictionary<string, PriceResponse?> priceResponse, IDictionary<string, string> symbolMap);
}

public class UserFavoriteAndPositionService : IUserFavoriteAndPositionService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserFavoriteAndPositionService> _logger;
    private readonly IStreamingDataCacheService _streamingDataCacheService;
    private readonly IOrderBookGetterService _orderBookGetterService;
    private readonly ILogoService _logoService;
    private readonly IFundMarketDataService _fundMarketDataService;
    private readonly InstrumentOrderOptions _options;

    public UserFavoriteAndPositionService(IHttpClientFactory httpClientFactory,
        ILogger<UserFavoriteAndPositionService> logger, IStreamingDataCacheService streamingDataCacheService,
        IOrderBookGetterService orderBookGetterService, ILogoService logoService,
        IFundMarketDataService fundMarketDataService, IOptions<InstrumentOrderOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient("UserFavoriteClient");
        _logger = logger;
        _streamingDataCacheService = streamingDataCacheService;
        _orderBookGetterService = orderBookGetterService;
        _logoService = logoService;
        _fundMarketDataService = fundMarketDataService;
        _options = options.Value;
    }

    // This is to get the user watchlist from Pi's API
    public async Task<UserFavoriteResponse> GetPiWatchListAsync(string userId, bool useMockWatchList = false)
    {
        try
        {
            if (useMockWatchList)
            {
                return new UserFavoriteResponse
                {
                    Data = new List<FavoriteItemResponse>
                    {
                        new FavoriteItemResponse
                        {
                            Symbol = "AAPL",
                            Venue = "NASDAQ"
                        },
                        new FavoriteItemResponse
                        {
                            Symbol = "CPALL",
                            Venue = "Equity"
                        }
                    }
                };
            }

            var endpoint = "internal/v1/watchlists";
            _httpClient.DefaultRequestHeaders.Add("user-id", userId);
            _logger.LogDebug("Request URL: {RequestUrl}", _httpClient.BaseAddress + endpoint);
            _logger.LogDebug("Request user-id: {UserId}", _httpClient.DefaultRequestHeaders.GetValues("user-id"));
            var response = await _httpClient.GetAsync(endpoint);
            _logger.LogDebug("Response: {Response}", response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserFavoriteResponse>()
                ?? new UserFavoriteResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user favorites for session {UserId}", userId);
            return new UserFavoriteResponse();
        }
    }

    public async Task<UserInstrumentFavoriteResponse> GetUserFavoritesAsyncAndEnhanceWithStreamingData(string userId, bool useMockWatchList = false, CancellationToken ct = default)
    {
        var userFavoriteResponse = await GetPiWatchListAsync(userId, useMockWatchList);

        var groupedFavorites = userFavoriteResponse.Data.GroupBy(x => string.Equals(x.Venue, "Fund", StringComparison.OrdinalIgnoreCase)).ToArray();

        var fundFavSymbols = groupedFavorites.FirstOrDefault(g => g.Key)?.Select(x => x.Symbol) ??
                             Enumerable.Empty<string>();
        var getFundSummaries = _fundMarketDataService.GetFundSummaries(fundFavSymbols, ct);

        var otherFavs =
            groupedFavorites.FirstOrDefault(g => !g.Key)?.Select(x => new FavoriteItemResponse
            { Symbol = x.Symbol, Venue = x.Venue }) ?? Enumerable.Empty<FavoriteItemResponse>();
        var getOtherSummaries = GetSymbolSummary(otherFavs, ct);

        await Task.WhenAll(getFundSummaries, getOtherSummaries);

        var instrumentSummaries = new List<InstrumentSummary>();

        var fundSummaries = await getFundSummaries;
        instrumentSummaries.AddRange(fundSummaries);

        var otherSummaries = await getOtherSummaries;
        instrumentSummaries.AddRange(otherSummaries);

        var orderConfig = _options.Sequence.ToList();
        var favoriteRes = instrumentSummaries
            .GroupBy(x => x.Category)
            .Select(group => new UserInstrumentFavoriteCategoryListResponse
            {
                Order = orderConfig.IndexOf(group.Key) + 1,
                InstrumentType = group.Select(y => y.Type).FirstOrDefault().ToString(),
                InstrumentCategory = group.Key.ToString(),
                InstrumentList = group.Select(y => MapToFavoriteItemResponse(y)).ToList()
            })
            .OrderBy(x => x.Order) // Add ordering by Order property
            .ToList();

        var response = new UserInstrumentFavoriteResponse
        {
            Code = "0",
            Message = "",
            Response = new UserInstrumentFavoriteDataResponse
            {
                Total = favoriteRes.Count,
                InstrumentCategoryList = favoriteRes
            }
        };

        return response;
    }

    public Task<UserInstrumentPositionResponse> GetUserPositionsAsyncAndEnhanceWithStreamingData(string userId)
    {
        //TODO Get positions from SET / TFEX / GE / MF
        var userInstrumentPositionDataResponse = new UserInstrumentPositionDataResponse
        {
            Total = 0,
            InstrumentCategoryList = []
        };
        return Task.FromResult(new UserInstrumentPositionResponse
        {
            Code = "0",
            Message = "",
            Response = userInstrumentPositionDataResponse
        });
    }

    private async Task<IEnumerable<InstrumentSummary>> GetSymbolSummary(IEnumerable<FavoriteItemResponse> favSymbols, CancellationToken ct)
    {
        var results = new List<InstrumentSummary>();
        foreach (var favoriteItem in favSymbols)
        {
            var findingKey = favoriteItem.Symbol;
            _logger.LogDebug("Watchlist.venue: '{Venue}'", favoriteItem.Venue);
            _logger.LogDebug("Watchlist.symbol: '{Symbol}'", favoriteItem.Symbol);
            var orderBook = await _orderBookGetterService.GetOrderBookBySymbol(favoriteItem.Symbol, favoriteItem.Venue);
            if (orderBook?.Status != "enabled")
                continue;
            if ((favoriteItem.Venue == "SET" || favoriteItem.Venue == "TFEX") && orderBook != null)
            {
                findingKey = orderBook.Symbol;
            }
            _logger.LogDebug("Finding key: '{FindingKey}'", findingKey);

            var currency = DetermineCurrency(favoriteItem.Venue, orderBook);
            var streamingData = (findingKey != null) ? await _streamingDataCacheService.GetStreamingDataAsync(findingKey) : new StreamingDataResponse();
            var logoSymbol = favoriteItem.Symbol;
            var category = orderBook?.Category;
            if (!string.IsNullOrEmpty(orderBook?.UnderlyingSymbol))
            {
                logoSymbol = orderBook.UnderlyingSymbol;
                category = null;
            }

            results.Add(new InstrumentSummary
            {
                Venue = favoriteItem.Venue,
                Symbol = favoriteItem.Symbol,
                FriendlyName = orderBook?.FriendlyName ?? string.Empty,
                Logo = _logoService.GetLogoUrl(favoriteItem.Venue, logoSymbol ?? string.Empty, category ?? string.Empty),
                Price = decimal.TryParse(streamingData.Price, out decimal resultPrice) ? resultPrice : 0,
                PriceChangeRatio = decimal.TryParse(streamingData.PriceChangedRate, out decimal resultPriceChangeRatio) ? resultPriceChangeRatio : 0,
                Currency = currency,
                Type = IntrumentExtension.GetInstrumentType(orderBook?.Type),
                Category = IntrumentExtension.GetInstrumentCategory(orderBook?.Category)
            });
        }

        return results;
    }

    private static string DetermineCurrency(string venue, SearchInstrumentDocument? orderBook)
    {
        if (orderBook != null)
        {
            return orderBook.Currency ?? "";
        }
        return venue switch
        {
            "Equity" or "Derivative" => "THB",
            "HKEX" => "HKD",
            _ => "",
        };
    }

    private UserInstrumentFavoriteItemResponse MapToFavoriteItemResponse(InstrumentSummary summary)
    {
        var response = new UserInstrumentFavoriteItemResponse
        {
            Symbol = summary.Symbol,
            Venue = summary.Venue,
            FriendlyName = summary.FriendlyName,
            Logo = summary.Logo,
            Currency = summary.Currency,
            IsFavorite = true,
            Price = summary.Price.ToString(CultureInfo.InvariantCulture),
            PriceChangeRatio = summary.PriceChangeRatio.ToString(CultureInfo.InvariantCulture),
            Nav = string.Empty,
            NavChange = string.Empty,
            NavChangePercentage = string.Empty
        };

        if (summary.Type == InstrumentType.Fund)
        {
            response.Nav = summary.Price.ToString(CultureInfo.InvariantCulture);
            response.NavChange = summary.PriceChange.ToString(CultureInfo.InvariantCulture);
            response.NavChangePercentage = summary.PriceChangeRatio.ToString(CultureInfo.InvariantCulture);
        }

        return response;
    }

    public List<string> MapCacheKeys(UserInstrumentFavoriteResponse response)
    {
        var setSymbols = response.Response.InstrumentCategoryList   // TODO: need to change from Symbol to OrderBookId
            .SelectMany(category => category.InstrumentList)
            .Where(instrument =>
                DomainVenue.SetVenue.Contains(instrument.Venue)
                || DomainVenue.TfexVenue.Contains(instrument.Venue)
            )
            .Select(instrument => instrument.Symbol)
            .ToList();
        var geSymbols = response.Response.InstrumentCategoryList
            .SelectMany(category => category.InstrumentList)
            .Where(instrument => DomainVenue.GeVenue.Contains(instrument.Venue))
            .Select(instrument => instrument.Symbol)
            .ToList();

        var cacheKeys = new List<string>();
        cacheKeys.AddRange(setSymbols.Select(symbol => $"{CacheKey.SetStreamingBody}{symbol}").ToList());   // TODO: need to change from Symbol to OrderBookId
        cacheKeys.AddRange(geSymbols.Select(symbol => $"{CacheKey.GeStreamingBody}{symbol}").ToList());
        return cacheKeys;
    }

    public UserInstrumentFavoriteResponse MapPrice(
        UserInstrumentFavoriteResponse response,
        IDictionary<string, PriceResponse?> priceResponse,
        IDictionary<string, string> symbolMap
    )
    {
        foreach (var category in response.Response.InstrumentCategoryList)
        {
            foreach (var instrument in category.InstrumentList)
            {
                var symbol = instrument.Symbol;
                var venue = instrument.Venue;
                PriceResponse? price = null;

                if (DomainVenue.SetVenue.Contains(venue)
                || DomainVenue.TfexVenue.Contains(venue))
                {
                    var cacheKey = symbolMap.TryGetValue(symbol, out var key) ? key : null;
                    price = priceResponse.TryGetValue(cacheKey ?? "", out var value) ? value : null;
                }
                else if (DomainVenue.GeVenue.Contains(venue))
                {
                    price = priceResponse.TryGetValue($"{CacheKey.GeStreamingBody}{symbol}", out var value) ? value : null;
                }

                if (price != null)
                {
                    instrument.Price = price.Price ?? "0.00";
                    instrument.PriceChangeRatio = price.PriceChangedRate ?? "0.00";
                }
            }
        }
        return response;
    }
}
