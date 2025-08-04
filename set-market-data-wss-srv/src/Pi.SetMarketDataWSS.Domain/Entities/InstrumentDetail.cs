namespace Pi.SetMarketDataWSS.Domain.Entities;

public class InstrumentDetail
{
    public int InstrumentDetailId { get; set; }
    public int InstrumentId { get; set; }
    public string? MarketCapitalization { get; set; }
    public string? ShareFreeFloat { get; set; }
    public string? Industry { get; set; }
    public string? Sector { get; set; }
    public string? PriceToEarningsRatio { get; set; }
    public string? PriceToBookRatio { get; set; }
    public string? PriceToSalesRatio { get; set; }
    public string? DividendYieldPercent { get; set; }
    public string? ExDividendDate { get; set; }
    public string? Units { get; set; }
    public string? Source { get; set; }
    public string? UnderlyingSymbol { get; set; }
    public bool IsClickable { get; set; }
    public string? UnderlyingVenue { get; set; }
    public string? UnderlyingInstrumentType { get; set; }
    public string? UnderlyingInstrumentCategory { get; set; }
    public string? UnderlyingOrderBookID { get; set; }
    public string? UnderlyingLogo { get; set; }
    public string? ExerciseRatio { get; set; }
    public string? ExerciseDate { get; set; }
    public int DaysToExercise { get; set; }
    public string? Direction { get; set; }
    public string? LastTradingDate { get; set; }
    public int DaysToLastTrade { get; set; }
    public string? IssuerSeries { get; set; }
    public string? ForeignCurrency { get; set; }
    public string? ConversionRatio { get; set; }
    public string? UnderlyingPrice { get; set; }
    public string? OpenInterest { get; set; }
    public string? ContractSize { get; set; }
    public string? LastNavPerShare { get; set; }
    public string? Objective { get; set; }
    public string? AssetClassFocus { get; set; }
    public string? ExpenseRatioPercentage { get; set; }
    public string? StrikePrice { get; set; }
    public string? TheoreticalPrice { get; set; }
    public string? ImpliedVolatilityPercent { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? MarketSegment { get; set; }
    public int MarketCode { get; set; }
    public int Decimals { get; set; }
    public int DecimalInStrikePrice { get; set; }
    public int DecimalInContractSizePQF { get; set; }

    public virtual Instrument Instrument { get; set; }
}