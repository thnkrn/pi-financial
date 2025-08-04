using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketData.Domain.Entities;

[BsonIgnoreExtraElements]
public class CuratedMember
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; }
    [BsonElement("curated_list_id")] public int CuratedListId { get; set; }

    [BsonElement("instrument_id")] public int InstrumentId { get; set; }

    [BsonElement("symbol")] public string? Symbol { get; set; }

    [BsonElement("ordering")] public int? Ordering { get; set; }

    [BsonElement("is_default")] public int? IsDefault { get; set; }

    [BsonElement("group_name")] public string? GroupName { get; set; }

    [BsonElement("sub_group_name")] public string? SubGroupName { get; set; }
}