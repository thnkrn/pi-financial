using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketData.Domain.Entities;

public class CuratedFilter
{
    [BsonId][BsonElement("_id")] public ObjectId Id { get; set; }
    [BsonIgnore] public string IdString => Id.ToString();

    [BsonElement("filter_id")] public int FilterId { get; set; }

    [BsonElement("filter_name")] public string? FilterName { get; set; }

    [BsonElement("filter_category")] public string? FilterCategory { get; set; }

    [BsonElement("filter_type")] public string? FilterType { get; set; }

    [BsonElement("category_priority")] public int CategoryPriority { get; set; }

    [BsonElement("group_name")] public string? GroupName { get; set; }

    [BsonElement("sub_group_name")] public string? SubGroupName { get; set; }

    [BsonElement("curated_list_id")] public int? CuratedListId { get; set; }

    [BsonElement("is_default")] public int IsDefault { get; set; }

    [BsonElement("highlight")] public int Highlight { get; set; }

    [BsonElement("ordering")] public int Ordering { get; set; }
}