namespace Pi.SetMarketDataWSS.Domain.Entities;

public class NavList
{
    public int NavListId { get; set; }
    public int InstrumentId { get; set; }
    public double PriceChange { get; set; }
    public double PriceChangeRatio { get; set; }
    public int FirstCandleTime { get; set; }
    public int NavTime { get; set; }
    public double Nav { get; set; }

    public virtual Instrument Instrument { get; set; }
}