using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketData.Domain.Entities;

public class MorningStarEtfs
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; }

    [BsonElement("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [BsonElement("exchange_id")]
    public string ExchangeId { get; set; } = string.Empty;

    [BsonElement("isin")]
    public string Isin { get; set; } = string.Empty;

    [BsonElement("market_cap")]
    public string MarketCap { get; set; } = string.Empty;

    [BsonElement("latest_nav")]
    public string LatestNAV { get; set; } = string.Empty;

    [BsonElement("dividend")]
    public string Dividend { get; set; } = string.Empty;

    [BsonElement("ex_dividend_date")]
    public string ExDividendDate { get; set; } = string.Empty;

    [BsonElement("expense_ratio")]
    public string ExpenseRatio { get; set; } = string.Empty;

    [BsonElement("currency")]
    public string Currency { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("website")]
    public string Website { get; set; } = string.Empty;

    [BsonElement("asset_class_focus")]
    public string AssetClassFocus { get; set; } = string.Empty;

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("manager")]
    public string Manager { get; set; } = string.Empty;
}
