using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Response;

public class InstrumentFavouriteCategoryList
{
    [JsonPropertyName("order")] public int Order { get; set; }

    [JsonPropertyName("instrumentType")] public string? InstrumentType { get; set; }

    [JsonPropertyName("instrumentCategory")]
    public string? InstrumentCategory { get; set; }

    [JsonPropertyName("instrumentList")] public List<InstrumentFavouriteList>? InstrumentList { get; set; }
}

public class InstrumentFavouriteList
{
    [JsonPropertyName("venue")] public string? Venue { get; set; }

    [JsonPropertyName("symbol")] public string? Symbol { get; set; }

    [JsonPropertyName("friendlyName")] public string? FriendlyName { get; set; }

    [JsonPropertyName("logo")] public string? Logo { get; set; }

    [JsonPropertyName("price")] public string? Price { get; set; }

    [JsonPropertyName("priceChangeRatio")] public string? PriceChangeRatio { get; set; }

    [JsonPropertyName("isFavorite")] public bool IsFavorite { get; set; }

    [JsonPropertyName("currency")] public string? Currency { get; set; }
}

public class InstrumentFavouriteResponse
{
    [JsonPropertyName("total")] public int Total { get; set; }

    [JsonPropertyName("instrumentCategoryList")]
    public List<InstrumentFavouriteCategoryList>? InstrumentCategoryList { get; set; }
}

public class UserInstrumentFavouriteResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public InstrumentFavouriteResponse? Response { get; set; }

    [JsonPropertyName("debugStack")] public string? DebugStack { get; set; }
}