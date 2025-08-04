namespace Pi.SetMarketDataWSS.Domain.Entities;

public class FundTradeDate
{
    public int FundTradeDateId { get; set; }
    public int InstrumentId { get; set; }
    public string? TradableDate { get; set; }

    public virtual Instrument Instrument { get; set; }
}