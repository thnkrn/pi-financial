using System.Text.Json.Serialization;

namespace Pi.MarketData.Domain.Models.Response;

public class InstrumentList
{
    [JsonPropertyName("order")]
    public int Order { get; set; }

    [JsonPropertyName("instrumentType")]
    public string? InstrumentType { get; set; }

    [JsonPropertyName("instrumentCategory")]
    public string? InstrumentCategory { get; set; }

    [JsonPropertyName("venue")]
    public string? Venue { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("friendlyName")]
    public string? FriendlyName { get; set; }

    [JsonPropertyName("logo")]
    public string? Logo { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("price")]
    public string? Price { get; set; }

    [JsonPropertyName("priceChange")]
    public string? PriceChange { get; set; }

    [JsonPropertyName("priceChangeRatio")]
    public string? PriceChangeRatio { get; set; }

    [JsonPropertyName("totalValue")]
    public string? TotalValue { get; set; }

    [JsonPropertyName("totalVolume")]
    public string? TotalVolume { get; set; }
}

public class InstrumentsResponse
{
    [JsonPropertyName("instrumentList")]
    public List<InstrumentList>? InstrumentList { get; set; }
}

public class HomeInstrumentsResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("response")]
    public InstrumentsResponse? Response { get; set; }

    [JsonPropertyName("debugStack")]
    public string? DebugStack { get; set; }
}