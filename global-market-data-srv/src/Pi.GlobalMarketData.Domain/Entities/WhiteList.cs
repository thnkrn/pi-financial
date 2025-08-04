using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketData.Domain.Entities;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class WhiteList
{
    [BsonId] [BsonRepresentation(BsonType.ObjectId)] [BsonElement("_id")] 
    public string Id { get; set; }

    [BsonElement("white_list_id")] 
    public int WhiteListId { get; set; }

    [BsonElement("ge_instrument_id")] 
    public int InstrumentId { get; set; }

    [BsonElement("symbol")] 
    public string? Symbol { get; set; }

    [BsonElement("exchange")] 
    public string? Exchange { get; set; }

    [BsonElement("mic")] 
    public string? Mic { get; set; }

    [BsonElement("standard_ticker")] 
    public string? StandardTicker { get; set; }

    [BsonElement("is_whitelist")] 
    public bool IsWhitelist { get; set; } = false;

    [BsonElement("instance_config_profile")] 
    public string? InstanceConfigProfile { get; set; }

    [BsonElement("ge_instrument")] 
    public GeInstrument? GeInstrument { get; set; }
}