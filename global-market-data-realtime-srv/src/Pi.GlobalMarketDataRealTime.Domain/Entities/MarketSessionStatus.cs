using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi.GlobalMarketDataRealTime.Domain.Entities;
public class MarketSessionStatus
{
    [BsonId][BsonElement("_id")] 
    public ObjectId Id { get; set; }

    [BsonElement("exchange")] 
    public string? Exchange { get; set; }

    [BsonElement("marketsession")] 
    public string? MarketSession { get; set; }

    [BsonElement("utcstarttime")] 
    public DateTime? UTCStartTime { get; set; }

    [BsonElement("utcendtime")] 
    public DateTime? UTCEndTime { get; set; }
}
