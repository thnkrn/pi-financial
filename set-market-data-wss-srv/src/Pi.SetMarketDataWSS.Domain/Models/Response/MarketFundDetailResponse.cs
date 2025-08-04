using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Response;

public class FundPerformance
{
    [JsonPropertyName("1d")] public string? _1d { get; set; }

    [JsonPropertyName("3m")] public string? _3m { get; set; }

    [JsonPropertyName("6m")] public string? _6m { get; set; }

    [JsonPropertyName("1y")] public string? _1y { get; set; }

    [JsonPropertyName("3y")] public string? _3y { get; set; }

    [JsonPropertyName("5y")] public string? _5y { get; set; }
}

public class FundDetailResponse
{
    [JsonPropertyName("inceptionData")] public string? InceptionData { get; set; }

    [JsonPropertyName("totalExpense")] public string? TotalExpense { get; set; }

    [JsonPropertyName("minimumPurchase")] public string? MinimumPurchase { get; set; }

    [JsonPropertyName("additionalPurchase")]
    public string? AdditionalPurchase { get; set; }

    [JsonPropertyName("settlementPeriod")] public string? SettlementPeriod { get; set; }

    [JsonPropertyName("divident")] public string? Divident { get; set; }

    [JsonPropertyName("fundPolicy")] public string? FundPolicy { get; set; }

    [JsonPropertyName("fxRiskPolicy")] public string? FxRiskPolicy { get; set; }

    [JsonPropertyName("updatedDate")] public string? UpdatedDate { get; set; }

    [JsonPropertyName("fundPerformance")] public FundPerformance? FundPerformance { get; set; }

    [JsonPropertyName("minBuyAmount")] public string? MinBuyAmount { get; set; }

    [JsonPropertyName("minSellAmount")] public string? MinSellAmount { get; set; }

    [JsonPropertyName("minSellUnit")] public string? MinSellUnit { get; set; }

    [JsonPropertyName("minHoldAmount")] public string? MinHoldAmount { get; set; }

    [JsonPropertyName("minHoldUnit")] public string? MinHoldUnit { get; set; }

    [JsonPropertyName("tradeStartHrs")] public string? TradeStartHrs { get; set; }

    [JsonPropertyName("tradeEndHrs")] public string? TradeEndHrs { get; set; }

    [JsonPropertyName("factSheetURL")] public string? FactSheetUrl { get; set; }

    [JsonPropertyName("investmentList")] public List<object>? InvestmentList { get; set; }

    [JsonPropertyName("allowSwitchOut")] public bool AllowSwitchOut { get; set; }
}

public class MarketFundDetailResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public FundDetailResponse? Response { get; set; }

    [JsonPropertyName("debugStack")] public string? DebugStack { get; set; }
}