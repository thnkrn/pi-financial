using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities
{
    public class Intermission
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("instrument_type")]
        public string? InstrumentType { get; set; }

        [BsonElement("from")]
        public TimeOnly? From { get; set; }

        [BsonElement("to")]
        public TimeOnly? To { get; set; }
    }
}