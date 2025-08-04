using System.Text.Json.Serialization;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Domain.Models.Response;

public class ProfileOverviewResponse
{
    [JsonPropertyName("market")]
    public string? Market { get; set; }

    [JsonPropertyName("exchange")]
    public string? Exchange { get; set; }

    [JsonPropertyName("exchangeTime")]
    public string? ExchangeTime { get; set; }

    [JsonPropertyName("lastPrice")]
    public string? LastPrice { get; set; }

    [JsonPropertyName("priorClose")]
    public string? PriorClose { get; set; }

    [JsonPropertyName("priceChange")]
    public string? PriceChange { get; set; }

    [JsonPropertyName("priceChangePercentage")]
    public string? PriceChangePercentage { get; set; }

    [JsonPropertyName("minimumOrderSize")]
    public string? MinimumOrderSize { get; set; }

    [JsonPropertyName("high52W")]
    public string? High52W { get; set; }

    [JsonPropertyName("low52W")]
    public string? Low52W { get; set; }

    [JsonPropertyName("contractMonth")]
    public string? ContractMonth { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("corporateActions")]
    public List<Entities.ProfileOverview.CorporateActionResponse>? CorporateActions { get; set; }

    [JsonPropertyName("tradingSign")]
    public List<string>? TradingSign { get; set; }
}

public class MarketProfileOverviewResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("response")]
    public ProfileOverviewResponse? Response { get; set; }
}