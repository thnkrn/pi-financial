using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketData.Domain.Entities;

public class PriceInfo
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("symbol")] public string? Symbol { get; set; }

    [BsonElement("entry_date")] public DateTime? EntryDate { get; set; }

    [BsonElement("price_info_id")] public int PriceInfoId { get; set; }

    [BsonElement("instrument_id")] public int InstrumentId { get; set; }

    [BsonElement("price")] public string? Price { get; set; }

    [BsonElement("currency")] public string? Currency { get; set; }

    [BsonElement("auction_price")] public string? AuctionPrice { get; set; }

    [BsonElement("auction_volume")] public string? AuctionVolume { get; set; }

    [BsonElement("open")] public string? Open { get; set; }

    [BsonElement("high_24h")] public string? High24H { get; set; }

    [BsonElement("low_24h")] public string? Low24H { get; set; }

    [BsonElement("high_52w")] public string? High52W { get; set; }

    [BsonElement("low_52w")] public string? Low52W { get; set; }

    [BsonElement("price_changed")] public string? PriceChanged { get; set; }

    [BsonElement("price_changed_rate")] public string? PriceChangedRate { get; set; }

    [BsonElement("volume")] public string? Volume { get; set; }

    [BsonElement("amount")] public string? Amount { get; set; }

    [BsonElement("change_amount")] public string? ChangeAmount { get; set; }

    [BsonElement("change_volume")] public string? ChangeVolume { get; set; }

    [BsonElement("turnover_rate")] public string? TurnoverRate { get; set; }

    [BsonElement("open2")] public string? Open2 { get; set; }

    [BsonElement("ceiling")] public string? Ceiling { get; set; }

    [BsonElement("floor")] public string? Floor { get; set; }

    [BsonElement("average")] public string? Average { get; set; }

    [BsonElement("average_buy")] public string? AverageBuy { get; set; }

    [BsonElement("average_sell")] public string? AverageSell { get; set; }

    [BsonElement("aggressor")] public string? Aggressor { get; set; }

    [BsonElement("pre_close")] public string? PreClose { get; set; }

    [BsonElement("status")] public string? Status { get; set; }

    [BsonElement("yield")] public string? Yield { get; set; }

    [BsonElement("pe")] public string? Pe { get; set; }

    [BsonElement("pb")] public string? Pb { get; set; }

    [BsonElement("total_amount")] public string? TotalAmount { get; set; }

    [BsonElement("total_amount_k")] public string? TotalAmountK { get; set; }

    [BsonElement("total_volume")] public string? TotalVolume { get; set; }

    [BsonElement("total_volume_k")] public string? TotalVolumeK { get; set; }

    [BsonElement("tradable_equity")] public string? TradableEquity { get; set; }

    [BsonElement("tradable_amount")] public string? TradableAmount { get; set; }

    [BsonElement("eps")] public string? Eps { get; set; }

    [BsonElement("last_trade")] public string? LastTrade { get; set; }

    [BsonElement("to_last_trade")] public int ToLastTrade { get; set; }

    [BsonElement("moneyness")] public string? Moneyness { get; set; }

    [BsonElement("maturity_date")] public string? MaturityDate { get; set; }

    [BsonElement("exercise_price")] public string? ExercisePrice { get; set; }

    [BsonElement("intrinsic_value")] public string? IntrinsicValue { get; set; }

    [BsonElement("p_settle")] public string? PSettle { get; set; }

    [BsonElement("poi")] public string? Poi { get; set; }

    [BsonElement("underlying")] public string? Underlying { get; set; }

    [BsonElement("open0")] public string? Open0 { get; set; }

    [BsonElement("open1")] public string? Open1 { get; set; }

    [BsonElement("basis")] public string? Basis { get; set; }

    [BsonElement("settle")] public string? Settle { get; set; }

    [BsonElement("instrument")] public virtual Instrument? Instrument { get; set; }
}