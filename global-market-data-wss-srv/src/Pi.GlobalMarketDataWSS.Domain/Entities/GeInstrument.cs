using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Pi.GlobalMarketDataWSS.Domain.Entities;

public class InvestmentType
{
    private InvestmentType(string value)
    {
        Value = value;
    }

    public string Value { get; }
    public static InvestmentType EQ => new("EQ");
    public static InvestmentType CE => new("CE");

    public override string ToString()
    {
        return Value;
    }
}

public class StockDetail
{
    private StockDetail(string value)
    {
        Value = value;
    }

    public string Value { get; }
    public static StockDetail ETFs => new("Global ETFs");
    public static StockDetail Stock => new("Global Stocks");
    public static StockDetail MorningStar => new("MorningStar");

    public override string ToString()
    {
        return Value;
    }
}

public class SymbolType
{
    private SymbolType(string value)
    {
        Value = value;
    }

    public string Value { get; }
    public static SymbolType Stock => new("STOCK");

    public override string ToString()
    {
        return Value;
    }
}

public class IdentifierType
{
    private IdentifierType(string value)
    {
        Value = value;
    }

    public string Value { get; }
    public static IdentifierType Isin => new("ISIN");
    public static IdentifierType ExchangeId => new("ExchangeId");

    public override string ToString()
    {
        return Value;
    }
}
public class GeInstrument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("ge_instrument_id")]
    public int GeInstrumentId { get; set; }

    [BsonElement("symbol")]
    public string? Symbol { get; set; }

    [BsonElement("exchange")]
    public string? Exchange { get; set; }

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("symbol_type")]
    public string? SymbolType { get; set; }

    [BsonElement("currency")]
    public string? Currency { get; set; }

    [BsonElement("country")]
    public string? Country { get; set; }

    [BsonElement("figi")]
    public string? Figi { get; set; }

    [BsonElement("isin")]
    public string? Isin { get; set; }

    [BsonElement("mic")]
    public string? Mic { get; set; }

    [BsonElement("standard_ticker")]
    public string? StandardTicker { get; set; }

    [BsonElement("investment_type")]
    public string? InvestmentType { get; set; }

    [BsonElement("morning_star_stock_status")]
    public string? MorningStarStockStatus { get; set; }

    [BsonElement("morning_star_suspend_flag")]
    public string? MorningStarSuspendFlag { get; set; }

    // To Add More Fields
    [BsonElement("instrument_type")]
    public string? InstrumentType { get; set; }

    [BsonElement("instrument_category")]
    public string? InstrumentCategory { get; set; }

    [BsonElement("venue")]
    public string? Venue { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("exchangeid_ms")]
    public string? ExchangeIdMs { get; set; }
}
