using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.MarketData.Domain.Entities;

public class CorporateAction
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("corporate_action_id")] public int CorporateActionId { get; set; }

    [BsonElement("instrument_id")] public int InstrumentId { get; set; }

    [BsonElement("type")] public string? Type { get; set; }

    [BsonElement("date")] public string? Date { get; set; }

    [BsonElement("code")] public string? Code { get; set; }

    [BsonElement("instrument")] public Instrument? Instrument { get; set; }
}