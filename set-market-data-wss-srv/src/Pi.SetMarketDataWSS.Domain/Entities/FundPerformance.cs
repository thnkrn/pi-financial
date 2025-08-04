namespace Pi.SetMarketDataWSS.Domain.Entities;

public class FundPerformance
{
    public int FundPerformanceId { get; set; }
    public int InstrumentId { get; set; }
    public string? _1d { get; set; }
    public string? _3m { get; set; }
    public string? _6m { get; set; }
    public string? _1y { get; set; }
    public string? _3y { get; set; }
    public string? _5y { get; set; }

    public virtual Instrument Instrument { get; set; }
}