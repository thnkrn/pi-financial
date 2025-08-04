using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketDataWSS.Domain.Entities;

public class Filter
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("filter_id")] public int FilterId { get; set; }

    [BsonElement("filter_name")] public string? FilterName { get; set; }

    [BsonElement("is_high_light")] public bool IsHighLight { get; set; }

    [BsonElement("is_default")] public bool IsDefault { get; set; }

    [BsonElement("order")] public int Order { get; set; }

    [BsonElement("filter_category")] public string? FilterCategory { get; set; }

    [BsonElement("filter_type")] public string? FilterType { get; set; }

    [BsonElement("support_secondary_filter")]
    public bool SupportSecondaryFilter { get; set; }
}