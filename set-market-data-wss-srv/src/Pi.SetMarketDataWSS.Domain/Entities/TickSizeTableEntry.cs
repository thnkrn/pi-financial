using System.Collections.Generic;

namespace Pi.SetMarketDataWSS.Domain.Entities;

public class TickSizeTable
{
    public int OrderBookId { get; set; }
    public List<TickSizeTableEntry> Entries { get; set; } = new List<TickSizeTableEntry>();
}

public class TickSizeTableEntry
{
    public int? TickSizeId { get; set; }
    public int? OrderBookId { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public decimal? TickSize { get; set; }
}