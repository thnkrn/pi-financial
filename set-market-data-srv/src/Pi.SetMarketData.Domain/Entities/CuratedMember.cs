using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities
{
    public class CuratedMember
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonIgnore]
        public string IdString => Id.ToString();

        [BsonElement("curated_list_id")]
        public int? CuratedListId { get; set; }

        [BsonElement("instrument_id")]
        public int? InstrumentId { get; set; }

        [BsonElement("symbol")]
        public string? Symbol { get; set; }

        [BsonElement("ordering")]
        public int? Ordering { get; set; }

        [BsonElement("is_default")]
        public int? IsDefault { get; set; }

        [BsonElement("group_name")]
        public string? GroupName { get; set; }

        [BsonElement("sub_group_name")]
        public string? SubGroupName { get; set; }
    }
}
