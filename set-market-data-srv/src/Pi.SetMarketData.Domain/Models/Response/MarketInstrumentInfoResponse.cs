using System.Text.Json.Serialization;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Domain.Models.Response;

public class InstrumentInfoResponse
{
    [JsonPropertyName("spreadSize")]
    public string? SpreadSize { get; set; }

    [JsonPropertyName("amountStepSize")]
    public string? AmountStepSize { get; set; }

    [JsonPropertyName("minimumPurchaseAmount")]
    public string? MinimumPurchaseAmount { get; set; }

    [JsonPropertyName("minimumPrice")]
    public string? MinimumPrice { get; set; }

    [JsonPropertyName("isNew")]
    public bool IsNew { get; set; }

    [JsonPropertyName("tradingSign")]
    public List<string>? TradingSign { get; set; }
}

public class MarketInstrumentInfoResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("response")]
    public InstrumentInfoResponse? Response { get; set; }
}