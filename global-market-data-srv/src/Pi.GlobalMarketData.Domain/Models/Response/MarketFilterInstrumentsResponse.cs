using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class InstrumentCategoryList
{
    [JsonProperty("order")]
    public int Order { get; set; }

    [JsonProperty("instrumentType")]
    public string? InstrumentType { get; set; }

    [JsonProperty("instrumentCategory")]
    public string? InstrumentCategory { get; set; }

    [JsonProperty("instrumentList")]
    public List<FilterInstrumentList>? InstrumentList { get; set; }
}

public class FilterInstrumentList
{
    [JsonProperty("venue")]
    public string? Venue { get; set; }

    [JsonProperty("symbol")]
    public string? Symbol { get; set; }

    [JsonProperty("friendlyName")]
    public string? FriendlyName { get; set; }

    [JsonProperty("logo")]
    public string? Logo { get; set; }

    [JsonProperty("price")]
    public string? Price { get; set; }

    [JsonProperty("priceChange")]
    public string? PriceChange { get; set; }

    [JsonProperty("priceChangeRatio")]
    public string? PriceChangeRatio { get; set; }

    [JsonProperty("isFavorite")]
    public bool IsFavorite { get; set; }

    [JsonProperty("unit")]
    public string? Unit { get; set; }

    [JsonProperty("latestNavTimestamp")]
    public int LatestNavTimestamp { get; set; }

    [JsonProperty("isMainSession")]
    public bool IsMainSession { get; set; }

    [JsonProperty("totalValue")]
    public string? TotalValue { get; set; }

    [JsonProperty("totalVolume")]
    public string? TotalVolume { get; set; }
    
    [JsonProperty("nav")]
    public string? Nav { get; set; }
        
    [JsonProperty("navChange")]
    public string? NavChange { get; set; }
        
    [JsonProperty("navChangePercentage")]
    public string? NavChangePercentage { get; set; }
}

public class FilterInstrumentsResponse
{
    [JsonProperty("instrumentCategoryList")]
    public List<InstrumentCategoryList>? InstrumentCategoryList { get; set; }
}

public class MarketFilterInstrumentsResponse
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("response")]
    public FilterInstrumentsResponse? Response { get; set; }
}