using System.ComponentModel.DataAnnotations.Schema;

namespace Pi.OnePort.Db2.Models;

public class AccountAvailable
{
    public required string AccountNo { get; set; }
    public string? TraderId { get; set; }
    public decimal? CreditLimit { get; set; }
    public decimal? BuyCredit { get; set; }
    public decimal? CashBalance { get; set; }
    [Column("AR")]
    public decimal? Ar { get; set; }
    [Column("AP")]
    public decimal? Ap { get; set; }
    [Column("AR_TRADE")]
    public decimal? ArTrade { get; set; }
    [Column("AP_TRADE")]
    public decimal? ApTrade { get; set; }
    [Column("TOTALBUY_MATCH")]
    public decimal? TotalBuyMatch { get; set; }
    [Column("TOTALBUY_UNMATCH")]
    public decimal? TotalBuyUnmatch { get; set; }
    [Column("CREDIT_TYPE")]
    public string? CreditType { get; set; }
}
