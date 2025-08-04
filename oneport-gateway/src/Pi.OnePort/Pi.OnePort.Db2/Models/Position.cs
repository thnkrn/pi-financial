using System.ComponentModel.DataAnnotations.Schema;

namespace Pi.OnePort.Db2.Models;

public abstract class Position
{
    public required string AccountNo { get; set; }
    public string? SecSymbol { get; set; }
    [Column("STOCK_TYPE")]
    public int? StockType { get; set; }
    [Column("STOCK_TYPE_CHAR")]
    public string? StockTypeChar { get; set; }
    public string? TrusteeId { get; set; }
    public decimal? StartVolume { get; set; }
    public decimal? StartPrice { get; set; }
    public decimal? AvaiVolume { get; set; }
    public decimal? ActualVolume { get; set; }
    public decimal? AvgPrice { get; set; }
    public decimal? TodayRealize { get; set; }
    public decimal? Amount { get; set; }
}
