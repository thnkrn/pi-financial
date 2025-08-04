using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.MarketData.Domain.Entities;

public class Instrument
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("order_book_id")] public int OrderBookId { get; set; }

    [BsonElement("instrument_type")] public string? InstrumentType { get; set; }

    [BsonElement("instrument_category")] public string? InstrumentCategory { get; set; }

    [BsonElement("venue")] public string? Venue { get; set; }

    [BsonElement("symbol")] public string? Symbol { get; set; }

    [BsonElement("friendly_name")] public string? FriendlyName { get; set; }

    [BsonElement("logo")] public string? Logo { get; set; }

    [BsonElement("security_type")] public string? SecurityType { get; set; }

    [BsonElement("exchange_timezone")] public string? ExchangeTimezone { get; set; }

    [BsonElement("currency")] public string? Currency { get; set; }

    [BsonElement("country")] public string? Country { get; set; }

    [BsonElement("market")] public string? Market { get; set; }

    [BsonElement("offset_seconds")] public int OffsetSeconds { get; set; }

    [BsonElement("is_projected")] public bool IsProjected { get; set; }

    [BsonElement("trading_unit")] public string? TradingUnit { get; set; }

    [BsonElement("min_bid_unit")] public string? MinBidUnit { get; set; }

    [BsonElement("multiplier")] public string? Multiplier { get; set; }

    [BsonElement("initial_margin")] public string? InitialMargin { get; set; }

    [BsonElement("exercise_ratio")] public string? ExerciseRatio { get; set; }

    [BsonElement("exercise_Date")] public string? ExerciseDate { get; set; }

    [BsonElement("exercise_price")] public string? ExercisePrice { get; set; }

    [BsonElement("conversion_ratio")] public string? ConversionRatio { get; set; }

    [BsonElement("days_to_exercise")] public string? DaysToExercise { get; set; }

    [BsonElement("last_trading_date")] public string? LastTradingDate { get; set; }

    [BsonElement("days_to_last_trade")] public string? DaysToLastTrade { get; set; }

    [BsonElement("issuer_series")] public string? IssuerSeries { get; set; }

    [BsonElement("price_infos")] public virtual ICollection<PriceInfo>? PriceInfos { get; set; }

    [BsonElement("order_books")] public virtual ICollection<OrderBook>? OrderBooks { get; set; }

    [BsonElement("public_trades")] public virtual ICollection<PublicTrade>? PublicTrades { get; set; }

    [BsonElement("instrument_details")] public virtual ICollection<InstrumentDetail>? InstrumentDetails { get; set; }

    [BsonElement("corporate_actions")] public virtual ICollection<CorporateAction>? CorporateActions { get; set; }

    [BsonElement("trading_signs")] public virtual ICollection<TradingSign>? TradingSigns { get; set; }

    [BsonElement("financials")] public virtual ICollection<Financial>? Financials { get; set; }

    [BsonElement("fund_performances")] public virtual ICollection<FundPerformance>? FundPerformances { get; set; }

    [BsonElement("fund_details")] public virtual ICollection<FundDetail>? FundDetails { get; set; }

    [BsonElement("nav_lists")] public virtual ICollection<NavList>? NavLists { get; set; }

    [BsonElement("fund_trade_dates")] public virtual ICollection<FundTradeDate>? FundTradeDates { get; set; }

    [BsonElement("indicators")] public virtual ICollection<Indicator>? Indicators { get; set; }

    [BsonElement("intermissions")] public virtual ICollection<Intermission>? Intermissions { get; set; }
}