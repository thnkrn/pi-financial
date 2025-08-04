using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class InstrumentList
{
    [JsonProperty("order")]
    public int Order { get; set; }

    [JsonProperty("instrumentType")]
    public string? InstrumentType { get; set; }

    [JsonProperty("instrumentCategory")]
    public string? InstrumentCategory { get; set; }

    [JsonProperty("venue")]
    public string? Venue { get; set; }

    [JsonProperty("symbol")]
    public string? Symbol { get; set; }

    [JsonProperty("friendlyName")]
    public string? FriendlyName { get; set; }

    [JsonProperty("logo")]
    public string? Logo { get; set; }

    [JsonProperty("unit")]
    public string? Unit { get; set; }

    [JsonProperty("price")]
    public string? Price { get; set; }

    [JsonProperty("priceChange")]
    public string? PriceChange { get; set; }

    [JsonProperty("priceChangeRatio")]
    public string? PriceChangeRatio { get; set; }

    [JsonProperty("totalValue")]
    public string? TotalValue { get; set; }

    [JsonProperty("totalVolume")]
    public string? TotalVolume { get; set; }
}

public class InstrumentsResponse
{
    [JsonProperty("instrumentList")]
    public List<InstrumentList>? InstrumentList { get; set; }
}

public class HomeInstrumentsResponse
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("response")]
    public InstrumentsResponse? Response { get; set; }
}