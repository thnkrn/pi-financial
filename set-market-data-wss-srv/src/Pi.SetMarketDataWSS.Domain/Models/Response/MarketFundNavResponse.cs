using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Response;

public class FundNavList
{
    [JsonPropertyName("symbol")] public string? Symbol { get; set; }

    [JsonPropertyName("priceChange")] public double PriceChange { get; set; }

    [JsonPropertyName("priceChangeRatio")] public double PriceChangeRatio { get; set; }

    [JsonPropertyName("firstCandleTime")] public int FirstCandleTime { get; set; }

    [JsonPropertyName("navList")] public List<List<double>>? NavList { get; set; }
}

public class FundNavResponse
{
    [JsonPropertyName("navList")] public List<FundNavList>? NavList { get; set; }
}

public class MarketFundNavResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public FundNavResponse? Response { get; set; }

    [JsonPropertyName("debugStack")] public string? DebugStack { get; set; }
}