using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.MarketData.Domain.Entities;

public class FundPerformance
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("fund_performance_id")] public int FundPerformanceId { get; set; }

    [BsonElement("instrument_id")] public int InstrumentId { get; set; }

    [BsonElement("1d")] public string? _1d { get; set; }

    [BsonElement("3m")] public string? _3m { get; set; }

    [BsonElement("6m")] public string? _6m { get; set; }

    [BsonElement("1y")] public string? _1y { get; set; }

    [BsonElement("3y")] public string? _3y { get; set; }

    [BsonElement("5y")] public string? _5y { get; set; }

    [BsonElement("instrument")] public Instrument? Instrument { get; set; }
}