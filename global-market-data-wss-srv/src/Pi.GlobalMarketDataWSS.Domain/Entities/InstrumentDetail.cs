using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketDataWSS.Domain.Entities;

public class InstrumentDetail
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("instrument_detail_id")] public int InstrumentDetailId { get; set; }

    [BsonElement("instrument_id")] public int InstrumentId { get; set; }

    [BsonElement("market_capitalization")] public string? MarketCapitalization { get; set; }

    [BsonElement("share_free_float")] public string? ShareFreeFloat { get; set; }

    [BsonElement("industry")] public string? Industry { get; set; }

    [BsonElement("sector")] public string? Sector { get; set; }

    [BsonElement("price_to_earnings_ratio")]
    public string? PriceToEarningsRatio { get; set; }

    [BsonElement("price_to_book_ratio")] public string? PriceToBookRatio { get; set; }

    [BsonElement("price_to_sales_ratio")] public string? PriceToSalesRatio { get; set; }

    [BsonElement("dividend_yield_percent")]
    public string? DividendYieldPercent { get; set; }

    [BsonElement("ex_dividend_date")] public string? ExDividendDate { get; set; }

    [BsonElement("units")] public string? Units { get; set; }

    [BsonElement("source")] public string? Source { get; set; }

    [BsonElement("underlying_symbol")] public string? UnderlyingSymbol { get; set; }

    [BsonElement("is_clickable")] public bool IsClickable { get; set; }

    [BsonElement("underlying_venue")] public string? UnderlyingVenue { get; set; }

    [BsonElement("underlying_instrument_type")]
    public string? UnderlyingInstrumentType { get; set; }

    [BsonElement("underlying_instrument_category")]
    public string? UnderlyingInstrumentCategory { get; set; }

    [BsonElement("underlying_logo")] public string? UnderlyingLogo { get; set; }

    [BsonElement("exercise_ratio")] public string? ExerciseRatio { get; set; }

    [BsonElement("exercise_date")] public string? ExerciseDate { get; set; }

    [BsonElement("days_to_exercise")] public int DaysToExercise { get; set; }

    [BsonElement("direction")] public string? Direction { get; set; }

    [BsonElement("last_trading_date")] public string? LastTradingDate { get; set; }

    [BsonElement("days_to_last_trade")] public int DaysToLastTrade { get; set; }

    [BsonElement("issuer_series")] public string? IssuerSeries { get; set; }

    [BsonElement("foreign_currency")] public string? ForeignCurrency { get; set; }

    [BsonElement("conversion_ratio")] public string? ConversionRatio { get; set; }

    [BsonElement("underlying_price")] public string? UnderlyingPrice { get; set; }

    [BsonElement("open_interest")] public string? OpenInterest { get; set; }

    [BsonElement("contract_size")] public string? ContractSize { get; set; }

    [BsonElement("last_nav_per_share")] public string? LastNavPerShare { get; set; }

    [BsonElement("objective")] public string? Objective { get; set; }

    [BsonElement("asset_class_focus")] public string? AssetClassFocus { get; set; }

    [BsonElement("expense_ratio_percentage")]
    public string? ExpenseRatioPercentage { get; set; }

    [BsonElement("strike_price")] public string? StrikePrice { get; set; }

    [BsonElement("theoretical_price")] public string? TheoreticalPrice { get; set; }

    [BsonElement("implied_volatility_percent")]
    public string? ImpliedVolatilityPercent { get; set; }

    [BsonElement("description")] public string? Description { get; set; }

    [BsonElement("website")] public string? Website { get; set; }

    [BsonElement("instrument")] public virtual Instrument? Instrument { get; set; }
}