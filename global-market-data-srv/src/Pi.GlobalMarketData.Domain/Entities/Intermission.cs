using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketData.Domain.Entities;

public class Intermission
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("intermission_id")] public int IntermissionId { get; set; }

    [BsonElement("instrument_id")] public int InstrumentId { get; set; }

    [BsonElement("from")] public int From { get; set; }

    [BsonElement("to")] public int To { get; set; }

    [BsonElement("instrument")] public virtual Instrument? Instrument { get; set; }
}