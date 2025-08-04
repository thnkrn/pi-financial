using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pi.GlobalMarketData.Domain.Entities;

public class RankingItem
{
    [Key, Column("bucket", Order = 0)]
    public DateTime Date { get; set; }

    [Key, Column("symbol", Order = 1)]
    public string? Symbol { get; set; }

    [Key, Column("venue", Order = 2)]
    public string? Venue { get; set; }

    [Column("amount")]
    public double AmountDouble { get; set; }

    [NotMapped]
    public decimal Amount => (decimal)AmountDouble;
}