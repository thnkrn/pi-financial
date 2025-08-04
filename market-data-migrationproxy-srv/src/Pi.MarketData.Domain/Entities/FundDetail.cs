using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.MarketData.Domain.Entities;

public class FundDetail
{
    [BsonId] [BsonElement("id")] public ObjectId Id { get; set; }

    [BsonElement("fund_detail_id")] public int FundDetailId { get; set; }

    [BsonElement("instrument_id")] public int InstrumentId { get; set; }

    [BsonElement("inception_date")] public string? InceptionDate { get; set; }

    [BsonElement("total_expense")] public string? TotalExpense { get; set; }

    [BsonElement("minimum_purchase")] public string? MinimumPurchase { get; set; }

    [BsonElement("additional_purchase")] public string? AdditionalPurchase { get; set; }

    [BsonElement("settlement_period")] public string? SettlementPeriod { get; set; }

    [BsonElement("dividend")] public string? Dividend { get; set; }

    [BsonElement("fund_policy")] public string? FundPolicy { get; set; }

    [BsonElement("fx_risk_policy")] public string? FxRiskPolicy { get; set; }

    [BsonElement("updated_date")] public string? UpdatedDate { get; set; }

    [BsonElement("min_buy_amount")] public string? MinBuyAmount { get; set; }

    [BsonElement("min_sell_amount")] public string? MinSellAmount { get; set; }

    [BsonElement("min_sell_unit")] public string? MinSellUnit { get; set; }

    [BsonElement("min_hold_amount")] public string? MinHoldAmount { get; set; }

    [BsonElement("min_hold_unit")] public string? MinHoldUnit { get; set; }

    [BsonElement("trade_start_hrs")] public string? TradeStartHrs { get; set; }

    [BsonElement("trade_end_hrs")] public string? TradeEndHrs { get; set; }

    [BsonElement("fact_sheet_url")] public string? FactSheetUrl { get; set; }

    [BsonElement("investment1")] public string? Investment1 { get; set; }

    [BsonElement("investment2")] public string? Investment2 { get; set; }

    [BsonElement("allow_switch_out")] public bool AllowSwitchOut { get; set; }

    [BsonElement("instrument")] public Instrument? Instrument { get; set; }
}