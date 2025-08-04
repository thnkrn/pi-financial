using System.ComponentModel.DataAnnotations.Schema;

namespace Pi.OnePort.Db2.Models;

public class AccountPositionCreditBalance : Position
{
    public decimal? LastSale { get; set; }
    [Column("MKT_VALUE")]
    public decimal? MktValue { get; set; }
    public decimal? MR { get; set; }
    public string? Grade { get; set; }
    [Column("AVG_COST")]
    public decimal? AvgCost { get; set; }
    public decimal? TodayMargin { get; set; }
    public decimal? StartClose { get; set; }
    public string? UpdateFlag { get; set; }
    public string? DelFlag { get; set; }
}
