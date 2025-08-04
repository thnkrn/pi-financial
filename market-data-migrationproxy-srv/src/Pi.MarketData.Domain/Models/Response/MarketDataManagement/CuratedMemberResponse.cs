using Newtonsoft.Json;

namespace Pi.MarketData.Domain.Models.Response.MarketDataManagement;

public class CuratedMemberResponse
{
    [JsonProperty("symbol")]
    public string? Symbol { get; set; }

    [JsonProperty("friendlyName")]
    public string? FriendlyName { get; set; }

    [JsonProperty("logo")]
    public string? Logo { get; set; }

    [JsonProperty("figi")]
    public string? Figi { get; set; }

    [JsonProperty("units")]
    public string? Units { get; set; }

    [JsonProperty("exchange")]
    public string? Exchange { get; set; }

    [JsonProperty("dataVendorCode")]
    public string? DataVendorCode { get; set; }
}