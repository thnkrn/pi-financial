namespace Pi.SetMarketDataWSS.Domain.Entities;

public class PublicTrade
{
    public int PublicTradeId { get; set; }
    public int InstrumentId { get; set; }
    public string? Price { get; set; }
    public string? Quantity { get; set; }
    public string? TradeTime { get; set; }

    public virtual Instrument Instrument { get; set; }
}