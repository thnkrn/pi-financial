using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketData.Domain.Entities;

public class MorningStarFlag
{
    [BsonId]
    [BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonElement("white_list_id")]
    public int WhiteListId { get; set; }

    [BsonElement("is_subscribe")]
    public bool IsSubscribe { get; set; } = false;

    [BsonElement("isin")]
    public string? Isin { get; set; }

    [BsonElement("exchange_id")]
    public string? ExchangeId { get; set; }

    [BsonElement("standard_ticker")]
    public string? StandardTicker { get; set; }

    [BsonElement("statement_type")]
    public string? StatementType { get; set; }

    [BsonElement("data_type")]
    public string? DataType { get; set; } = Models.Request.DataType.Restated.Value;

    [BsonElement("start_date")]
    public string? StartDate { get; set; }

    [BsonElement("end_date")]
    public string? EndDate { get; set; }

    [BsonElement("excluding_from")]
    public string? ExcludingFrom { get; set; }

    [BsonElement("excluding_to")]
    public string? ExcludingTo { get; set; }

    [BsonElement("instrument_category")]
    public string? InstrumentCategory { get; set; }
}
