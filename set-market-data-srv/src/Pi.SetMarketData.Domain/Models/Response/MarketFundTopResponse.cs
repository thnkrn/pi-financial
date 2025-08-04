using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Response;

public class TopInstrumentList
{
    [JsonPropertyName("instrumentType")]
    public string? InstrumentType { get; set; }

    [JsonPropertyName("fundCategory")]
    public string? FundCategory { get; set; }

    [JsonPropertyName("venue")]
    public string? Venue { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("friendlyName")]
    public string? FriendlyName { get; set; }

    [JsonPropertyName("logo")]
    public string? Logo { get; set; }

    [JsonPropertyName("nav")]
    public string? Nav { get; set; }

    [JsonPropertyName("returnPercentage")]
    public string? ReturnPercentage { get; set; }

    [JsonPropertyName("isFavorite")]
    public bool IsFavorite { get; set; }

    [JsonPropertyName("asOfDate")]
    public string? AsOfDate { get; set; }

    [JsonPropertyName("riskLevel")]
    public string? RiskLevel { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

public class FundTopResponse
{
    [JsonPropertyName("instrumentList")]
    public List<TopInstrumentList>? InstrumentList { get; set; }
}

public class MarketFundTopResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("response")]
    public FundTopResponse? Response { get; set; }
}