using Newtonsoft.Json;

namespace Pi.MarketData.Search.Domain.Models;

public class UserPositionResponse
{
    [JsonProperty("data")] public UserPositionDataResponse Data { get; set; } = new();
}

public class UserPositionDataResponse
{
    [JsonProperty("positions")] public List<UserPositionItemResponse> Positions { get; set; } = new();
}

public class UserPositionItemResponse
{
    [JsonProperty("symbol")] public string Symbol { get; set; } = string.Empty;
    [JsonProperty("venue")] public string Venue { get; set; } = string.Empty;
}

public class UserInstrumentPositionResponse
{
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("response")]
    public UserInstrumentPositionDataResponse Response { get; set; } = new();
}

public class UserInstrumentPositionDataResponse
{
    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("instrumentCategoryList")]
    public List<UserInstrumentPositionCategoryListResponse> InstrumentCategoryList { get; set; } = new();
}

public class UserInstrumentPositionCategoryListResponse
{
    [JsonProperty("order")]
    public int Order { get; set; }

    [JsonProperty("instrumentType")]
    public string InstrumentType { get; set; } = string.Empty;

    [JsonProperty("instrumentCategory")]
    public string Category { get; set; } = string.Empty;

    [JsonProperty("instrumentList")]
    public List<UserInstrumentPositionItemResponse> InstrumentList { get; set; } = new();
}

public class UserInstrumentPositionItemResponse
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