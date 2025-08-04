using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Response.MarketDataManagement;

public class CuratedMemberResponse
{
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("friendlyName")]
    public string? FriendlyName { get; set; }

    [JsonPropertyName("logo")]
    public string? Logo { get; set; }

    [JsonPropertyName("figi")]
    public string? Figi { get; set; }

    [JsonPropertyName("units")]
    public string? Units { get; set; }

    [JsonPropertyName("exchange")]
    public string? Exchange { get; set; }

    [JsonPropertyName("dataVendorCode")]
    public string? DataVendorCode { get; set; }

    [JsonPropertyName("dataVendorCode2")]
    public string? DataVendorCode2 { get; set; }
}