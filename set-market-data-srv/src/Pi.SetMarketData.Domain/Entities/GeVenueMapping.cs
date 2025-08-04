using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities
{
    public class GeVenueMapping
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string? Id { get; set; } // Stores the ObjectId as a string

        [BsonElement("source")]
        public string? Source { get; set; }

        [BsonElement("exchange")]
        public string? Exchange { get; set; }

        [BsonElement("mic")]
        public string? Mic { get; set; }

        [BsonElement("exchangeid_ms")]
        public string? ExchangeIdMs { get; set; }

        [BsonElement("venue_code")]
        public string? VenueCode { get; set; }
    }
}
