namespace Pi.SetMarketDataWSS.Domain.Entities;

public class FundDetail
{
    public int FundDetailId { get; set; }
    public int InstrumentId { get; set; }
    public string? InceptionDate { get; set; }
    public string? TotalExpense { get; set; }
    public string? MinimumPurchase { get; set; }
    public string? AdditionalPurchase { get; set; }
    public string? SettlementPeriod { get; set; }
    public string? Dividend { get; set; }
    public string? FundPolicy { get; set; }
    public string? FxRiskPolicy { get; set; }
    public string? UpdatedDate { get; set; }
    public string? MinBuyAmount { get; set; }
    public string? MinSellAmount { get; set; }
    public string? MinSellUnit { get; set; }
    public string? MinHoldAmount { get; set; }
    public string? MinHoldUnit { get; set; }
    public string? TradeStartHrs { get; set; }
    public string? TradeEndHrs { get; set; }
    public string? FactSheetUrl { get; set; }
    public string? Investment1 { get; set; }
    public string? Investment2 { get; set; }
    public bool AllowSwitchOut { get; set; }

    public virtual Instrument Instrument { get; set; }
}