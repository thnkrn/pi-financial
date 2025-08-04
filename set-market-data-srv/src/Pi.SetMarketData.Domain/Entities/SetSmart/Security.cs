using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pi.SetMarketData.Domain.Entities;

[Table("SECURITY")]
public class Security
{
    [Key, Column("I_SECURITY")]
    public required int ISecurity { get; set; }

    [Column("I_COMPANY")]
    public required int ICompany { get; set; }

    [Column("I_MARKET")]
    public required char IMarket { get; set; }
    
    [Column("I_INDUSTRY")]
    public short? IIndustry { get; set; }
    
    [Column("I_SECTOR")]
    public short? ISector { get; set; }
    
    [Column("I_SUBSECTOR")]
    public short? ISubsector { get; set; }
    
    [Column("I_SEC_TYPE")]
    public char? ISecType { get; set; }
    
    [Column("N_SECURITY")]
    public string? NSecurity { get; set; }
    
    [Column("N_SECURITY_T")]
    public string? NSecurityT { get; set; }
    
    [Column("N_SECURITY_E")]
    public string? NSecurityE { get; set; }
    
    [Column("I_SECURITY_LOCAL")]
    public int? ISecurityELocal { get; set; }
    
    [Column("I_ISIN")]
    public string? IISIN { get; set; }
    
    [Column("I_NATIVE")]
    public char? INative { get; set; }
    
    [Column("F_FOREIGN_TRADE")]
    public string? FForeignTrade { get; set; }

    [Column("f_frac_trade")]
    public string? FFracTrade { get; set; }
    
}