using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pi.SetMarketData.Domain.Entities;

[Table("SECURITY_DETAIL")]
public class SecurityDetail
{
    [Key, Column("I_SECURITY")]
    public required int ISecurity { get; set; }

    [Column("Q_SHARE_AUTHORIZED")]
    public long? QShareAuthorized { get; set; }

    [Column("Q_SHARE_ISSUED")]
    public long? QShareIssued { get; set; }

    [Column("Q_SHARE_PAIDUP")]
    public long? QSharePaidup { get; set; }

    [Column("Q_SHARE_LISTED")]
    public long? QShareListed { get; set; }

    [Column("Q_RESERVE")]
    public long? QReserve { get; set; }

    [Column("Q_SHARE_INDEX")]
    public long? QShareIndex { get; set; }

    [Column("Q_ACC_CONVERT")]
    public long? QAccConvert { get; set; }

    [Column("Q_TREASURY")]
    public long? QTreasury { get; set; }

    [Column("Z_PAR")]
    public decimal? ZPar { get; set; }

    [Column("Z_MULTIPLIER")]
    public decimal? ZMultiplier { get; set; }

    [Column("Z_IPO")]
    public decimal? ZIpo { get; set; }

    [Column("Z_EXERCISE")]
    public decimal? ZExercise { get; set; }

    [Column("D_LISTED")]
    public DateTime? DListed { get; set; }

    [Column("D_DELISTED")]
    public DateTime? DDelisted { get; set; }

    [Column("D_FIRST_TRADE")]
    public DateTime? DFirstTrade { get; set; }

    [Column("D_LAST_TRADE")]
    public DateTime? DLastTrade { get; set; }

    [Column("D_FIRST_ISSUED")]
    public DateTime? DFirstIssued { get; set; }

    [Column("D_EXPIRED")]
    public DateTime? DExpired { get; set; }

    [Column("D_FISCAL_END")]
    public string? DFiscalEnd { get; set; }

    [Column("D_BEG_SUBSCRIPTION")]
    public DateTime? DBegSubscription { get; set; }
    
    [Column("D_END_SUBSCRIPTION")]
    public DateTime? DEndSubscription { get; set; }

    [Column("D_FIRST_PAID")]
    public DateTime? DFirstPaid { get; set; }

    [Column("D_LAST_PAID")]
    public DateTime? DLastPaid { get; set; }

    [Column("Q_FIRST_RATIO")]
    public double? QFirstRatio { get; set; }

    [Column("Q_LAST_RATIO")]
    public double? QLastRatio { get; set; }

    [Column("I_ACCT_FORM")]
    public string? IAcctForm { get; set; }

    [Column("I_COLLATERAL")]
    public string? ICollateral { get; set; }

    [Column("I_STYLE_OPTION")]
    public string? IStyleOption { get; set; }

    [Column("I_TYPE_OPTION")]
    public string? ITypeOption { get; set; }

    [Column("I_TYPE_ISSUER")]
    public string? ITypeIssuer { get; set; }

    [Column("I_TYPE_COLLATERAL")]
    public string? ITypeCollateral { get; set; }

    [Column("I_TYPE_PRICE")]
    public string? ITypePrice { get; set; }

    [Column("P_COUPON")]
    public decimal? PCoupon { get; set; }

    [Column("F_LISTED")]
    public string? FListed { get; set; }

    [Column("F_INTEREST")]
    public string? FInterest { get; set; }

    [Column("N_CURRENCY")]
    public string? NCurrency { get; set; }

    [Column("N_FILE_T")]
    public string? NFileT { get; set; }

    [Column("N_FILE_E")]
    public string? NFileE { get; set; }

    [Column("D_LAST_EXERCISE")]
    public DateTime? DLastExercise { get; set; }

    [Column("D_FIRST_EXERCISE")]
    public DateTime? DFirstExercise { get; set; }

    [Column("D_AUTO_EXERCISE")]
    public DateTime? DAutoExercise { get; set; }

    [Column("F_DELISTED")]
    public string? FDelisted { get; set; }

    [Column("F_INFRA_FUND")]
    public string? FInfraFund { get; set; }

    [Column("Q_TTM")]
    public int? QTtm { get; set; }

    [Column("E_URL")]
    public string? EUrl { get; set; }

    [Column("Q_WORKING_TTM")]
    public int? QWorkingTtm { get; set; }

    [Column("I_SUB_SEC_TYPE")]
    public string? ISubSecType { get; set; }

    [Column("I_TYPE_OFFERING")]
    public string? ITypeOffering { get; set; }

    [Column("F_FRAC_TRADE")]
    public string? FFracTrade { get; set; }

    [Column("I_TRADING_SESSION")]
    public string? ITradingSession { get; set; }

    [Column("I_TYPE_SETTLEMENT")]
    public string? ITypeSettlement { get; set; }
}