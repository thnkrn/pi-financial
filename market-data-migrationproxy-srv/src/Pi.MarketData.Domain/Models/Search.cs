using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.MarketData.Domain.Models;

public class GeInstrumentDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    [BsonElement("symbol")] public required string Symbol { get; set; }
    [BsonElement("name")] public required string Name { get; set; }
    [BsonElement("instrument_type")] public required string Type { get; set; }
    [BsonElement("currency")] public required string Currency { get; set; }
    [BsonElement("instrument_category")] public required string Category { get; set; }
    [BsonElement("friendly_name")] public required string FriendlyName { get; set; }
    [BsonElement("exchange")] public required string Exchange { get; set; }
    [BsonElement("status")] public required string Status { get; set; }
    [BsonElement("venue")] public required string Venue { get; set; }
}
public class SetTfexInstrumentDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }
    [BsonElement("symbol")] public required string Symbol { get; set; }
    [BsonElement("long_name")] public required string Name { get; set; }
    [BsonElement("instrument_type")] public required string Type { get; set; }
    [BsonElement("currency")] public required string Currency { get; set; }
    [BsonElement("instrument_category")] public required string Category { get; set; }
    [BsonElement("friendly_name")] public required string FriendlyName { get; set; }
    [BsonElement("exchange")] public required string Exchange { get; set; }
    [BsonElement("status")] public required string Status { get; set; }
    [BsonElement("venue")] public required string Venue { get; set; }
    [BsonElement("order_book_id")]
    public required int OrderBookId { get; set; }
}

public class SearchInstrumentDocument
{
    [JsonProperty("_id")] public required string Id { get; set; }
    [JsonProperty("symbol")] public required string Symbol { get; set; }
    [JsonProperty("name")] public required string Name { get; set; }
    [JsonProperty("instrument_type")] public required string Type { get; set; }
    [JsonProperty("currency")] public required string Currency { get; set; }
    [JsonProperty("instrument_category")] public required string Category { get; set; }
    [JsonProperty("friendly_name")] public required string FriendlyName { get; set; }
    [JsonProperty("status")] public required string Status { get; set; }
    [JsonProperty("venue")] public required string Venue { get; set; }
    [JsonProperty("order_book_id")] public required string OrderBookId { get; set; } = string.Empty;
}

public class SearchInstrumentResponse
{
    [JsonProperty("data")] public required List<SearchInstrumentGroupResponse> Data { get; set; }
}

public class SearchInstrumentGroupResponse
{
    [JsonProperty("type")] public required string Type { get; set; }
    [JsonProperty("category")] public required string Category { get; set; }
    [JsonProperty("order")] public required int Order { get; set; }
    [JsonProperty("instruments")] public required List<SearchInstrumentItemResponse> Instruments { get; set; }
}

public class SearchInstrumentItemResponse
{
    [JsonProperty("venue")] public required string Venue { get; set; }
    [JsonProperty("symbol")] public required string Symbol { get; set; }
    [JsonProperty("name")] public required string Name { get; set; }
    [JsonProperty("friendlyName")] public required string FriendlyName { get; set; }
    [JsonProperty("logo")] public required string Logo { get; set; }
    [JsonProperty("price")] public required string Price { get; set; }
    [JsonProperty("priceChange")] public required string PriceChange { get; set; }
    [JsonProperty("priceChangeRatio")] public required string PriceChangeRatio { get; set; }

    [JsonProperty("nav")] public string? Nav { get; set; }
    [JsonProperty("navChange")] public string? NavChange { get; set; }
    [JsonProperty("navChangePercentage")] public string? NavChangePercentage { get; set; }

    [JsonProperty("isFavorite")] public required bool IsFavorite { get; set; }
    [JsonProperty("unit")] public required string Unit { get; set; }
    [JsonProperty("type")] public required string Type { get; set; }
    [JsonProperty("category")] public required string Category { get; set; }
    [JsonProperty("orderBookId")] public string OrderBookId { get; set; } = string.Empty;
}