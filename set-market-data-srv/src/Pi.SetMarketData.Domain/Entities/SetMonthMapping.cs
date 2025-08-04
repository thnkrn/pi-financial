using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities
{
    public class SetMonthMapping
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("month_code")]
        public string? MonthCode { get; set; }

        [BsonElement("month_abbr")]
        public string? MonthAbbr { get; set; }

    }
}
