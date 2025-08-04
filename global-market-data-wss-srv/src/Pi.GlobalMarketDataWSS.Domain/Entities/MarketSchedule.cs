using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketDataWSS.Domain.Entities;

public static class MarketSession
{
    public const string Offline = "Offline";
    public const string PreMarket = "PreMarket";
    public const string MainSession = "MainSession";
    public const string AfterMarket = "AfterMarket";
    //NOTE: ScheduleBreak for HKEX Market
    public const string ScheduleBreak = "ScheduleBreak";
}

public class MarketSchedule
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("symbol")] public string? Symbol { get; set; }
    [BsonElement("exchange")] public string? Exchange { get; set; }

    [BsonElement("marketsession")] public string? MarketSession { get; set; }

    [BsonElement("utcstarttime")] public DateTime? UTCStartTime { get; set; }

    [BsonElement("utcendtime")] public DateTime? UTCEndTime { get; set; }
}