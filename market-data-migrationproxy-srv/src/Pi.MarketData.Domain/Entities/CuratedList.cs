using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.MarketData.Domain.Entities;

public class CuratedList
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("curated_list_id")] public int CuratedListId { get; set; }

    [BsonElement("curated_list_code")] public string? CuratedListCode { get; set; }

    [BsonElement("curated_type")] public string? CuratedType { get; set; }

    [BsonElement("relevant_to")] public string? RelevantTo { get; set; }

    [BsonElement("list_name")] public string? ListName { get; set; }

    [BsonElement("name")] public string? Name { get; set; }

    [BsonElement("hashtag")] public string? Hashtag { get; set; }

    [BsonElement("ordering")] public int? Ordering { get; set; }

    [BsonElement("create_time")] public string? CreateTime { get; set; }

    [BsonElement("update_time")] public string? UpdateTime { get; set; }

    [BsonElement("update_by")] public string? UpdateBy { get; set; }

    [BsonElement("symbols")] public List<string>? Symbols { get; set; }   
}