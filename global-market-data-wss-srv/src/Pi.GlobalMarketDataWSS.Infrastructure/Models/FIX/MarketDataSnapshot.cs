namespace Pi.GlobalMarketDataWSS.Infrastructure.Models.FIX;

public class MarketDataEntry
{
    //public int MDEntryType { get; set; }
    public string
        MDEntryType
    {
        get;
        set;
    } // Oum Arcadia 20-Jun-24: Change to string type because MDEntryType not just a number only

    public decimal MDEntryPx { get; set; }
    public decimal MDEntrySize { get; set; }
    public DateTime? MDEntryDate { get; set; }
    public DateTime? MDEntryTime { get; set; }
}

public class MarketDataSnapshot
{
    public string Symbol { get; set; }
    public string MDReqID { get; set; }
    public List<MarketDataEntry> Entries { get; set; } = new();
}