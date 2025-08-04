using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketDataRealTime.Domain.Entities;

public class GeInstrument
{
    [BsonId]
    [BsonElement("_id")]
    public ObjectId Id { get; set; }

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
}
