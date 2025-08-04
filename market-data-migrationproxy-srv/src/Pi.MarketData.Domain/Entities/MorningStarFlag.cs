using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.MarketData.Domain.Entities;

public class MorningStarFlag
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("morning_star_flag_id")] public int MorningStarFlagId { get; set; }

    [BsonElement("white_list_id")] public int WhiteListId { get; set; }

    [BsonElement("mic")] public string? Mic { get; set; }

    [BsonElement("standard_ticker")] public string? StandardTicker { get; set; }

    [BsonElement("white_list")] public WhiteList? WhiteList { get; set; }
}