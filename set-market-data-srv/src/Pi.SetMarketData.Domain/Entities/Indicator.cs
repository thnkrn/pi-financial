using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities
{
    public class Indicator
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("indicator_id")]
        public int IndicatorId { get; set; }

        [BsonElement("instrument_id")]
        public int InstrumentId { get; set; }

        [BsonElement("candle_type")]
        public string? CandleType { get; set; }

        [BsonElement("first_candle_time")]
        public int FirstCandleTime { get; set; }

        [BsonElement("candle_time")]
        public int CandleTime { get; set; }

        [BsonElement("open")]
        public double Open { get; set; }

        [BsonElement("high")]
        public double High { get; set; }

        [BsonElement("low")]
        public double Low { get; set; }

        [BsonElement("close")]
        public double Close { get; set; }

        [BsonElement("volume")]
        public double Volume { get; set; }

        [BsonElement("boll_upper")]
        public double BollUpper { get; set; }

        [BsonElement("boll_middle")]
        public double BollMiddle { get; set; }

        [BsonElement("boll_lower")]
        public double BollLower { get; set; }

        [BsonElement("ema")]
        public double Ema { get; set; }

        [BsonElement("kdj_k")]
        public double KdjK { get; set; }

        [BsonElement("kdj_d")]
        public double KdjD { get; set; }

        [BsonElement("kdj_j")]
        public double KdjJ { get; set; }

        [BsonElement("ma")]
        public double Ma { get; set; }

        [BsonElement("macd_macd")]
        public double MacdMacd { get; set; }

        [BsonElement("macd_signal")]
        public double MacdSignal { get; set; }

        [BsonElement("macd_hist")]
        public double MacdHist { get; set; }

        [BsonElement("rsi")]
        public double Rsi { get; set; }

        [BsonElement("instrument")]
        public virtual Instrument? Instrument { get; set; }
    }
}