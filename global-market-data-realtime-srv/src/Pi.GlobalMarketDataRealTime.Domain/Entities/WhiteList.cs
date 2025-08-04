using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketDataRealTime.Domain.Entities;

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class WhiteList
{
    [BsonId] 
    [BsonElement("_id")] 
    public ObjectId Id { get; set; }

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
    public bool? IsWhitelist { get; set; }

    [BsonElement("instance_config_profile")]
    public string? InstanceConfigProfile { get; set; }

    [BsonElement("ge_instrument")] 
    public GeInstrument? GeInstrument { get; set; }
}