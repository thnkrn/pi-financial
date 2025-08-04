namespace Pi.SetMarketDataWSS.Domain.Entities;

public class TradingSign
{
    public int TradingSignId { get; set; }
    public int InstrumentId { get; set; }
    public string? Sign { get; set; }

    public virtual Instrument Instrument { get; set; }
}