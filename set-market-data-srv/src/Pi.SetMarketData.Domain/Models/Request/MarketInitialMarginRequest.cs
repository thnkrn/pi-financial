using Newtonsoft.Json;

namespace Pi.SetMarketData.Domain.Models.Request;

public class MarketInitialMarginRequest
{
    public DateTime AsOfDate { get; set; }
    public List<InitialMarginData> Data { get; set; } = [];
}

public class InitialMarginData
{
    [JsonProperty("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonProperty("productType")]
    public string ProductType { get; set; } = string.Empty;

    [JsonProperty("im")]
    public string Im { get; set; } = string.Empty;
}
