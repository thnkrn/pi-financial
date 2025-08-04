using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Response;

public class FundTradeDateResponse
{
    [JsonPropertyName("tradableDate")]
    public List<string>? TradableDate { get; set; }
}

public class MarketFundTradeDateResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("response")]
    public FundTradeDateResponse? Response { get; set; }
}