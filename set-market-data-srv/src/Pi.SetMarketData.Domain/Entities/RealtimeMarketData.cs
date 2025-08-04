using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pi.SetMarketData.Domain.Entities
{
    [Table("realtime_market_data")]
    public class RealtimeMarketData
    {
        [Key, Column("date_time", Order = 0)]
        public DateTimeOffset DateTime { get; set; }

        [Key, Column("symbol", Order = 1)]
        public string? Symbol { get; set; }

        [Key, Column("venue", Order = 2)]
        public string? Venue { get; set; }
        [Column("price")]
        public double? Price { get; set; }
        [Column("volume")]
        public int? Volume { get; set; }
        [Column("amount")]
        public double? Amount { get; set; }
    }
}