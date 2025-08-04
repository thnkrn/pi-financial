using Newtonsoft.Json;

namespace Pi.MarketData.Domain.Models;

public class UserFavoriteResponse
{
    [JsonProperty("data")] public List<FavoriteItemResponse> Data { get; set; } = new();
}

public class FavoriteItemResponse
{
    [JsonProperty("symbol")] public string Symbol { get; set; } = string.Empty;
    [JsonProperty("venue")] public string Venue { get; set; } = string.Empty;
}

public class UserInstrumentFavoriteResponse
{
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("response")]
    public UserInstrumentFavoriteDataResponse Response { get; set; } = new();
}

public class UserInstrumentFavoriteDataResponse
{
    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("instrumentCategoryList")]
    public List<UserInstrumentFavoriteCategoryListResponse> InstrumentCategoryList { get; set; } = new();
}

public class UserInstrumentFavoriteCategoryListResponse
{
    [JsonProperty("order")]
    public int Order { get; set; }

    [JsonProperty("instrumentType")]
    public string InstrumentType { get; set; } = string.Empty;

    [JsonProperty("instrumentCategory")]
    public string Category { get; set; } = string.Empty;

    [JsonProperty("instrumentList")]
    public List<UserInstrumentFavoriteItemResponse> InstrumentList { get; set; } = new();
}

public class UserInstrumentFavoriteItemResponse
{
    [JsonProperty("venue")]
    public string Venue { get; set; } = string.Empty;

    [JsonProperty("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonProperty("friendlyName")]
    public string FriendlyName { get; set; } = string.Empty;

    [JsonProperty("logo")]
    public string Logo { get; set; } = string.Empty;

    [JsonProperty("price")]
    public string Price { get; set; } = string.Empty;

    [JsonProperty("priceChange")]
    public string PriceChange { get; set; } = string.Empty;

    [JsonProperty("priceChangeRatio")]
    public string PriceChangeRatio { get; set; } = string.Empty;

    [JsonProperty("nav")] public string? Nav { get; set; }
    [JsonProperty("navChange")] public string? NavChange { get; set; }
    [JsonProperty("navChangePercentage")] public string? NavChangePercentage { get; set; }

    [JsonProperty("isFavorite")]
    public bool IsFavorite { get; set; } = true;

    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;
}