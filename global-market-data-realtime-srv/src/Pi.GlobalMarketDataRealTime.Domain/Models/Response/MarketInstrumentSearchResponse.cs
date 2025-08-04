using Newtonsoft.Json;

namespace Pi.GlobalMarketDataRealTime.Domain.Models.Response;

public class InstrumentSearchCategoryList
{
    [JsonProperty("order")] public int Order { get; set; }

    [JsonProperty("instrumentType")] public string? InstrumentType { get; set; }

    [JsonProperty("instrumentCategory")] public string? InstrumentCategory { get; set; }

    [JsonProperty("instrumentList")] public List<InstrumentSearchList>? InstrumentList { get; set; }
}

public class InstrumentSearchList
{
    [JsonProperty("venue")] public string? Venue { get; set; }

    [JsonProperty("symbol")] public string? Symbol { get; set; }

    [JsonProperty("friendlyName")] public string? FriendlyName { get; set; }

    [JsonProperty("logo")] public string? Logo { get; set; }

    [JsonProperty("price")] public string? Price { get; set; }

    [JsonProperty("priceChange")] public string? PriceChange { get; set; }

    [JsonProperty("priceChangeRatio")] public string? PriceChangeRatio { get; set; }

    [JsonProperty("isFavorite")] public bool IsFavorite { get; set; }

    [JsonProperty("unit")] public string? Unit { get; set; }
}

public class InstrumentSearchResponse
{
    [JsonProperty("instrumentCategoryList")]
    public List<InstrumentSearchCategoryList>? InstrumentCategoryList { get; set; }
}

public class MarketInstrumentSearchResponse
{
    [JsonProperty("code")] public string? Code { get; set; }

    [JsonProperty("message")] public string? Message { get; set; }

    [JsonProperty("response")] public InstrumentSearchResponse? Response { get; set; }

    [JsonProperty("debugStack")] public string? DebugStack { get; set; }
}