using System.Text.Json.Serialization;

namespace Pi.MarketData.Domain.Models.Response;

public class ProfileFundamentalsResponse
{
    [JsonPropertyName("marketCapitalization")]
    public string? MarketCapitalization { get; set; }

    [JsonPropertyName("shareFreeFloat")] public string? ShareFreeFloat { get; set; }

    [JsonPropertyName("industry")] public string? Industry { get; set; }

    [JsonPropertyName("sector")] public string? Sector { get; set; }

    [JsonPropertyName("country")] public string? Country { get; set; }

    [JsonPropertyName("priceToEarningsRatio")]
    public string? PriceToEarningsRatio { get; set; }

    [JsonPropertyName("priceToBookRatio")] public string? PriceToBookRatio { get; set; }

    [JsonPropertyName("priceToSalesRatio")]
    public string? PriceToSalesRatio { get; set; }

    [JsonPropertyName("dividendYieldPercentage")]
    public string? DividendYieldPercentage { get; set; }

    [JsonPropertyName("exDividendDate")] public string? ExDividendDate { get; set; }

    [JsonPropertyName("units")] public string? Units { get; set; }

    [JsonPropertyName("source")] public string? Source { get; set; }

    [JsonPropertyName("underlyingSymbol")] public string? UnderlyingSymbol { get; set; }

    [JsonPropertyName("isClickable")] public bool IsClickable { get; set; }

    [JsonPropertyName("underlyingVenue")] public string? UnderlyingVenue { get; set; }

    [JsonPropertyName("underlyingInstrumentType")]
    public string? UnderlyingInstrumentType { get; set; }

    [JsonPropertyName("underlyingInstrumentCategory")]
    public string? UnderlyingInstrumentCategory { get; set; }

    [JsonPropertyName("underlyingLogo")] public string? UnderlyingLogo { get; set; }

    [JsonPropertyName("exerciseRatio")] public string? ExerciseRatio { get; set; }

    [JsonPropertyName("exercisePrice")] public string? ExercisePrice { get; set; }

    [JsonPropertyName("exerciseDate")] public string? ExerciseDate { get; set; }

    [JsonPropertyName("daysToExercise")] public string? DaysToExercise { get; set; }

    [JsonPropertyName("moneyness")] public string? Moneyness { get; set; }

    [JsonPropertyName("direction")] public string? Direction { get; set; }

    [JsonPropertyName("multiplier")] public string? Multiplier { get; set; }

    [JsonPropertyName("lastTradingDate")] public string? LastTradingDate { get; set; }

    [JsonPropertyName("daysToLastTrade")] public string? DaysToLastTrade { get; set; }

    [JsonPropertyName("maturityDate")] public string? MaturityDate { get; set; }

    [JsonPropertyName("issuerSeries")] public string? IssuerSeries { get; set; }

    [JsonPropertyName("foreignCurrency")] public string? ForeignCurrency { get; set; }

    [JsonPropertyName("conversionRatio")] public string? ConversionRatio { get; set; }

    [JsonPropertyName("underlyingPrice")] public string? UnderlyingPrice { get; set; }

    [JsonPropertyName("basis")] public string? Basis { get; set; }

    [JsonPropertyName("openInterest")] public string? OpenInterest { get; set; }

    [JsonPropertyName("contractSize")] public string? ContractSize { get; set; }

    [JsonPropertyName("lastNavPerShare")] public string? LastNavPerShare { get; set; }

    [JsonPropertyName("objective")] public string? Objective { get; set; }

    [JsonPropertyName("assetClassFocus")] public string? AssetClassFocus { get; set; }

    [JsonPropertyName("expenseRatioPercentage")]
    public string? ExpenseRatioPercentage { get; set; }

    [JsonPropertyName("strikePrice")] public string? StrikePrice { get; set; }

    [JsonPropertyName("theoreticalPrice")] public string? TheoreticalPrice { get; set; }

    [JsonPropertyName("intrinsicValue")] public string? IntrinsicValue { get; set; }

    [JsonPropertyName("impliedVolatilityPercentage")]
    public string? ImpliedVolatilityPercentage { get; set; }
}

public class MarketProfileFundamentalsResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public ProfileFundamentalsResponse? Response { get; set; }

    [JsonPropertyName("debugStack")] public string? DebugStack { get; set; }
}