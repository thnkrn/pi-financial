using Pi.MarketData.Domain.Models;

namespace Pi.MarketData.SearchAPI.Services;

public interface IUserFavoriteAndPositionService
{
    Task<UserFavoriteResponse> GetPiWatchListAsync(string sessionId, bool useMockWatchList = false);
    Task<UserInstrumentFavoriteResponse> GetUserFavoritesAsyncAndEnhanceWithStreamingData(string sessionId, bool useMockWatchList = false);
    Task<UserInstrumentPositionResponse> GetUserPositionsAsyncAndEnhanceWithStreamingData(string sessionId, bool useMockWatchList = false);
}

public class UserFavoriteAndPositionService : IUserFavoriteAndPositionService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserFavoriteAndPositionService> _logger;
    private readonly IStreamingDataCacheService _streamingDataCacheService;
    private readonly IOrderBookGetterService _orderBookGetterService;
    private readonly ILogoService _logoService;
    public UserFavoriteAndPositionService(IHttpClientFactory httpClientFactory, ILogger<UserFavoriteAndPositionService> logger, IStreamingDataCacheService streamingDataCacheService, IOrderBookGetterService orderBookGetterService, ILogoService logoService)
    {
        _httpClient = httpClientFactory.CreateClient("UserFavoriteClient");
        _logger = logger;
        _streamingDataCacheService = streamingDataCacheService;
        _orderBookGetterService = orderBookGetterService;
        _logoService = logoService;
    }

    // This is to get the user watchlist from Pi's API
    public async Task<UserFavoriteResponse> GetPiWatchListAsync(string sessionId, bool useMockWatchList = false)
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
                            Venue = "SET"
                        }
                    }
                };
            }

            var endpoint = "internal/v1/watchlists";
            _httpClient.DefaultRequestHeaders.Add("user-id", sessionId);
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
            _logger.LogError(ex, "Error fetching user favorites for session {SessionId}", sessionId);
            return new UserFavoriteResponse();
        }
    }

    public async Task<UserInstrumentFavoriteResponse> GetUserFavoritesAsyncAndEnhanceWithStreamingData(string sessionId, bool useMockWatchList = false)
    {
        var userFavoriteResponse = await GetPiWatchListAsync(sessionId, useMockWatchList);
        var categoryMapData = new Dictionary<string, UserInstrumentFavoriteCategoryListResponse>();
        foreach (var favoriteItem in userFavoriteResponse.Data)
        {
            var findingKey = favoriteItem.Symbol;
            var currency = "USD";
            _logger.LogDebug("Watchlist.venue: '{Venue}'", favoriteItem.Venue);
            _logger.LogDebug("Watchlist.symbol: '{Symbol}'", favoriteItem.Symbol);
            if (favoriteItem.Venue == "SET" || favoriteItem.Venue == "TFEX")
            {
                findingKey = await _orderBookGetterService.GetOrderBookIdBySymbol(favoriteItem.Symbol, favoriteItem.Venue);
                currency = "THB";
            }
            _logger.LogDebug("Finding key: '{FindingKey}'", findingKey);

            var streamingData = (findingKey != null) ? await _streamingDataCacheService.GetStreamingDataAsync(findingKey) : new StreamingDataResponse();

            if (!categoryMapData.ContainsKey(favoriteItem.Venue))
            {
                categoryMapData[favoriteItem.Venue] = new UserInstrumentFavoriteCategoryListResponse
                {
                    Category = favoriteItem.Venue,
                    InstrumentList = new List<UserInstrumentFavoriteItemResponse>()
                };
            }
            categoryMapData[favoriteItem.Venue].InstrumentList.Add(new UserInstrumentFavoriteItemResponse
            {
                Symbol = favoriteItem.Symbol,
                Venue = favoriteItem.Venue,
                FriendlyName = "",
                Logo = _logoService.GetLogoUrl(favoriteItem.Venue, favoriteItem.Symbol),
                Price = streamingData.Price,
                PriceChangeRatio = streamingData.PriceChangedRate,
                Currency = currency
            });
        }
        var categoryListResponse = new List<UserInstrumentFavoriteCategoryListResponse>();
        categoryMapData.Values.ToList().ForEach(category =>
        {
            categoryListResponse.Add(new UserInstrumentFavoriteCategoryListResponse
            {
                Order = 0,
                InstrumentType = category.InstrumentType,
                Category = category.Category,
                InstrumentList = category.InstrumentList
            });
        });
        var userInstrumentFavoriteDataResponse = new UserInstrumentFavoriteDataResponse
        {
            Total = categoryMapData.Count,
            InstrumentCategoryList = categoryListResponse
        };
        return new UserInstrumentFavoriteResponse
        {
            Code = "0000",
            Message = "Success",
            Response = userInstrumentFavoriteDataResponse
        };
    }

    public async Task<UserInstrumentPositionResponse> GetUserPositionsAsyncAndEnhanceWithStreamingData(string sessionId, bool useMockWatchList = false)
    {
        var userPositionResponse = await GetPiWatchListAsync(sessionId, useMockWatchList);
        var categoryMapData = new Dictionary<string, UserInstrumentPositionCategoryListResponse>();
        foreach (var positionItem in userPositionResponse.Data)
        {
            var findingKey = positionItem.Symbol;
            var currency = "USD";
            _logger.LogDebug("Position.venue: '{Venue}'", positionItem.Venue);
            _logger.LogDebug("Position.symbol: '{Symbol}'", positionItem.Symbol);
            if (positionItem.Venue == "SET" || positionItem.Venue == "TFEX")
            {
                findingKey = await _orderBookGetterService.GetOrderBookIdBySymbol(positionItem.Symbol, positionItem.Venue);
                currency = "THB";
            }
            _logger.LogDebug("Finding key: '{FindingKey}'", findingKey);

            var streamingData = (findingKey != null) ? await _streamingDataCacheService.GetStreamingDataAsync(findingKey) : new StreamingDataResponse();

            if (!categoryMapData.ContainsKey(positionItem.Venue))
            {
                categoryMapData[positionItem.Venue] = new UserInstrumentPositionCategoryListResponse
                {
                    Category = positionItem.Venue,
                    InstrumentList = new List<UserInstrumentPositionItemResponse>()
                };
            }
            categoryMapData[positionItem.Venue].InstrumentList.Add(new UserInstrumentPositionItemResponse
            {
                Symbol = positionItem.Symbol,
                Venue = positionItem.Venue,
                FriendlyName = "",
                Logo = _logoService.GetLogoUrl(positionItem.Venue, positionItem.Symbol),
                Price = streamingData.Price,
                PriceChangeRatio = streamingData.PriceChangedRate,
                Currency = currency
            });
        }
        var categoryListResponse = new List<UserInstrumentPositionCategoryListResponse>();
        categoryMapData.Values.ToList().ForEach(category =>
        {
            categoryListResponse.Add(new UserInstrumentPositionCategoryListResponse
            {
                Order = 0,
                InstrumentType = category.InstrumentType,
                Category = category.Category,
                InstrumentList = category.InstrumentList
            });
        });
        var userInstrumentPositionDataResponse = new UserInstrumentPositionDataResponse
        {
            Total = categoryMapData.Count,
            InstrumentCategoryList = categoryListResponse
        };
        return new UserInstrumentPositionResponse
        {
            Code = "0000",
            Message = "Success",
            Response = userInstrumentPositionDataResponse
        };
    }
}