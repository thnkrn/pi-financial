using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Response;

public class InstrumentSearchCategoryList
{
    [JsonPropertyName("order")] public int Order { get; set; }

    [JsonPropertyName("instrumentType")] public string? InstrumentType { get; set; }

    [JsonPropertyName("instrumentCategory")]
    public string? InstrumentCategory { get; set; }

    [JsonPropertyName("instrumentList")] public List<InstrumentSearchList>? InstrumentList { get; set; }
}

public class InstrumentSearchList
{
    [JsonPropertyName("venue")] public string? Venue { get; set; }

    [JsonPropertyName("symbol")] public string? Symbol { get; set; }

    [JsonPropertyName("friendlyName")] public string? FriendlyName { get; set; }

    [JsonPropertyName("logo")] public string? Logo { get; set; }

    [JsonPropertyName("price")] public string? Price { get; set; }

    [JsonPropertyName("priceChange")] public string? PriceChange { get; set; }

    [JsonPropertyName("priceChangeRatio")] public string? PriceChangeRatio { get; set; }

    [JsonPropertyName("isFavorite")] public bool IsFavorite { get; set; }

    [JsonPropertyName("unit")] public string? Unit { get; set; }
}

public class InstrumentSearchResponse
{
    [JsonPropertyName("instrumentCategoryList")]
    public List<InstrumentSearchCategoryList>? InstrumentCategoryList { get; set; }
}

public class MarketInstrumentSearchResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public InstrumentSearchResponse? Response { get; set; }

    [JsonPropertyName("debugStack")] public string? DebugStack { get; set; }
}