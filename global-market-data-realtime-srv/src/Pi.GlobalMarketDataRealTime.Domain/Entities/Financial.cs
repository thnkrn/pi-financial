using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketDataRealTime.Domain.Entities;

public class Financial
{
    [BsonId]
    [BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonElement("financial_id")]
    public int FinancialId { get; set; }

    [BsonElement("instrument_id")]
    public int InstrumentId { get; set; }

    [BsonElement("financial_type")]
    public string? FinancialType { get; set; }

    [BsonElement("yy")]
    public string? Yy { get; set; }

    [BsonElement("q1")]
    public string? Q1 { get; set; }

    [BsonElement("q2")]
    public string? Q2 { get; set; }

    [BsonElement("q3")]
    public string? Q3 { get; set; }

    [BsonElement("q4")]
    public string? Q4 { get; set; }

    [BsonElement("statement_type")]
    public string? StatementType { get; set; }

    [BsonElement("units")]
    public string? Units { get; set; }

    [BsonElement("latest_financials")]
    public string? LatestFinancials { get; set; }

    [BsonElement("source")]
    public string? Source { get; set; }

    [BsonElement("instrument")]
    public virtual Instrument Instrument { get; set; }
}
