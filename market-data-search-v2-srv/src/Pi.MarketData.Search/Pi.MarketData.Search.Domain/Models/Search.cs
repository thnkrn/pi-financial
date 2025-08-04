using System.Text.RegularExpressions;
using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.MarketData.Search.Domain.Models;

public class GeInstrumentDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("symbol")] public string? Symbol { get; set; }
    [BsonElement("name")] public string? Name { get; set; }
    [BsonElement("instrument_type")] public string? Type { get; set; }
    [BsonElement("currency")] public string? Currency { get; set; }
    [BsonElement("instrument_category")] public string? Category { get; set; }
    [BsonElement("friendly_name")] public string? FriendlyName { get; set; }
    [BsonElement("exchange")] public string? Exchange { get; set; }
    [BsonElement("status")] public string? Status { get; set; }
    [BsonElement("venue")] public string? Venue { get; set; }
}
public class SetTfexInstrumentDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    [BsonElement("symbol")] public string? Symbol { get; set; }
    [BsonElement("long_name")] public string? Name { get; set; }
    [BsonElement("instrument_type")] public string? Type { get; set; }
    [BsonElement("currency")] public string? Currency { get; set; }
    [BsonElement("instrument_category")] public string? Category { get; set; }
    [BsonElement("friendly_name")] public string? FriendlyName { get; set; }
    [BsonElement("exchange")] public string? Exchange { get; set; }
    [BsonElement("status")] public string? Status { get; set; }
    [BsonElement("venue")] public string? Venue { get; set; }
    [BsonElement("order_book_id")] public int OrderBookId { get; set; }
    [BsonElement("maturity_date")] public string? MaturityDate { get; set; }
    [BsonElement("last_trading_date")] public string? LastTradingDate { get; set; }
    [BsonElement("security_type")] public string? SecurityType { get; set; }
    [BsonElement("underlying_order_book_id")] public int? UnderlyingOrderBookId { get; set; }
    [BsonElement("exercise_Date")] public string? ExerciseDate { get; set; }
    [BsonElement("deprecated")] public bool? Deprecated { get; set; }
}

public class SearchInstrumentDocument
{
    [JsonProperty("_id")] public string? Id { get; set; }
    [JsonProperty("symbol")] public string? Symbol { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("instrument_type")] public string? Type { get; set; }
    [JsonProperty("currency")] public string? Currency { get; set; }
    [JsonProperty("instrument_category")] public string? Category { get; set; }
    [JsonProperty("friendly_name")] public string? FriendlyName { get; set; }
    [JsonProperty("status")] public string? Status { get; set; }
    [JsonProperty("venue")] public string? Venue { get; set; }
    [JsonProperty("order_book_id")] public string? OrderBookId { get; set; } = string.Empty;
    [JsonProperty("last_synced_at")] public string? LastSyncedAt { get; set; }
    [JsonProperty("custom_index")] public string? CustomIndex { get; set; } = string.Empty;
    [JsonProperty("underlying_symbol")] public string? UnderlyingSymbol { get; set; } = string.Empty;
    [JsonProperty("security_type")] public string? SecurityType { get; set; } = string.Empty;
    [JsonProperty("deprecated")] public bool? Deprecated { get; set; } = false;

    public void SetCustomIndex()
    {
        CustomIndex = string.Empty;
        if (FriendlyName == null) return;

        // e.g. JD.C -> JD C
        const string pattern = @"([a-zA-Z]{2})\.([a-zA-Z])";
        const string replacement = "$1 $2";
        var result = Regex.Replace(FriendlyName, pattern, replacement);

        if (result != FriendlyName)
        {
            CustomIndex = result;
        }
    }
}

public class SearchInstrumentResponse
{
    [JsonProperty("data")] public List<SearchInstrumentGroupResponse> Data { get; set; } = [];
}

public class SearchInstrumentGroupResponse
{
    [JsonProperty("type")] public string? Type { get; set; }
    [JsonProperty("category")] public string? Category { get; set; }
    [JsonProperty("order")] public int Order { get; set; }
    [JsonProperty("instruments")] public List<SearchInstrumentItemResponse> Instruments { get; set; } = [];
}

public class SearchInstrumentItemResponse
{
    [JsonProperty("venue")] public string? Venue { get; set; }
    [JsonProperty("symbol")] public string? Symbol { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("friendlyName")] public string? FriendlyName { get; set; }
    [JsonProperty("logo")] public string? Logo { get; set; }
    [JsonProperty("price")] public string? Price { get; set; }
    [JsonProperty("priceChange")] public string? PriceChange { get; set; }
    [JsonProperty("priceChangeRatio")] public string? PriceChangeRatio { get; set; }

    [JsonProperty("nav")] public string? Nav { get; set; }
    [JsonProperty("navChange")] public string? NavChange { get; set; }
    [JsonProperty("navChangePercentage")] public string? NavChangePercentage { get; set; }

    [JsonProperty("isFavorite")] public bool IsFavorite { get; set; }
    [JsonProperty("unit")] public string? Unit { get; set; }
    [JsonProperty("type")] public string? Type { get; set; }
    [JsonProperty("category")] public string? Category { get; set; }
    [JsonProperty("orderBookId")] public string OrderBookId { get; set; } = string.Empty;
    [JsonProperty("_es_score")] public string EsScore { get; set; } = string.Empty;
}
