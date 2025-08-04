using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Request;

public class MarketFundNavRequest
{
    [JsonPropertyName("fromTimestamp")]
    public int FromTimestamp { get; set; }

    [JsonPropertyName("symbolList")]
    public List<string>? SymbolList { get; set; }

    [JsonPropertyName("toTimestamp")]
    public int ToTimestamp { get; set; }
}