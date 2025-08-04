using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities
{
    public class CuratedList
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonIgnore]
        public string IdString => Id.ToString();

        [BsonElement("curated_list_id")]
        public int CuratedListId { get; set; }

        [BsonElement("curated_list_code")]
        public string? CuratedListCode { get; set; }

        [BsonElement("curated_type")]
        public string? CuratedType { get; set; }

        [BsonElement("relevant_to")]
        public string? RelevantTo { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("hashtag")]
        public string? Hashtag { get; set; }

        [BsonElement("ordering")]
        public int? Ordering { get; set; }

        [BsonElement("create_time")]
        public string? CreateTime { get; set; }

        [BsonElement("update_time")]
        public string? UpdateTime { get; set; }

        [BsonElement("update_by")]
        public string? UpdateBy { get; set; }

        [BsonElement("is_default")]
        public int? IsDefault { get; set; }
    }
}