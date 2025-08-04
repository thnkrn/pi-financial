using System.ComponentModel.DataAnnotations.Schema;

namespace Pi.OnePort.Db2.Models;

public class AccountAvailableCreditBalance
{
    [Column("CUST_ID")]
    public string? CustId { get; set; }
    public string? TraderId { get; set; }
    public decimal? CreditLimit { get; set; }
    public decimal? BuyCredit { get; set; }
    [Column("CASH_BALANCE")]
    public decimal? CashBalance { get; set; }
    public string? Pc { get; set; }
    public decimal? Asset { get; set; }
    public decimal? Liability { get; set; }
    public decimal? Equity { get; set; }
    public decimal? LMV { get; set; }
    public decimal? Collat { get; set; }
    public decimal? Debt { get; set; }
    public decimal? SMV { get; set; }
    public decimal? MR { get; set; }
    public decimal? BuyMR { get; set; }
    public decimal? SellMR { get; set; }
    public decimal? EE { get; set; }
    public decimal? PP { get; set; }
    [Column("CALL_MARGIN")]
    public decimal? CallMargin { get; set; }
    [Column("CALL_FORCE")]
    public decimal? CallForce { get; set; }
    [Column("BRK_CALL_LMV")]
    public decimal? BrkCallLMV { get; set; }
    [Column("BRK_CALL_SMV")]
    public decimal? BrkCallSMV { get; set; }
    [Column("BRK_SELL_LMV")]
    public decimal? BrkSellLMV { get; set; }
    [Column("BRK_SELL_SMV")]
    public decimal? BrkSellSMV { get; set; }
    public string? Action { get; set; }
    public decimal? BuyUnmatch { get; set; }
    public decimal? SellUnmatch { get; set; }
    public decimal? Ar { get; set; }
    public decimal? Ap { get; set; }
    [Column("AR_T1")]
    public decimal? ArT1 { get; set; }
    [Column("AP_T1")]
    public decimal? ApT1 { get; set; }
    [Column("AR_T2")]
    public decimal? ArT2 { get; set; }
    [Column("AP_T2")]
    public decimal? ApT2 { get; set; }
    public decimal? Withdrawal { get; set; }
    [Column("LMV_HAIRCUT")]
    public decimal? LMVHaircut { get; set; }
    [Column("EQUITY_HAIRCUT")]
    public decimal? EquityHaircut { get; set; }
    [Column("EE_1")]
    public decimal? EE1 { get; set; }
    [Column("EE_50")]
    public decimal? EE50 { get; set; }
    [Column("EE_60")]
    public decimal? EE60 { get; set; }
    [Column("EE_70")]
    public decimal? EE70 { get; set; }
    [Column("EE_MTM")]
    public decimal? EEMTM { get; set; }
    [Column("EE_MTM50")]
    public decimal? EEMTM50 { get; set; }
    [Column("EE_MTM60")]
    public decimal? EEMTM60 { get; set; }
    [Column("EE_MTM70")]
    public decimal? EEMTM70 { get; set; }
    public string? UpdateFlag { get; set; }
    public string? DelFlag { get; set; }
}
