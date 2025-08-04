namespace Pi.SetMarketDataWSS.Domain.Entities;

public class Instrument
{
    public int InstrumentId { get; set; }
    public string? InstrumentType { get; set; }
    public string? InstrumentCategory { get; set; }
    public string? Venue { get; set; }
    public string? Symbol { get; set; }
    public string? FriendlyName { get; set; }
    public string? Logo { get; set; }
    public string? SecurityType { get; set; }
    public string? ExchangeTimezone { get; set; }
    public string? Country { get; set; }
    public int OffsetSeconds { get; set; }
    public bool IsProjected { get; set; }
    public string? TradingUnit { get; set; }
    public string? MinBidUnit { get; set; }
    public string? Multiplier { get; set; }
    public string? InitialMargin { get; set; }

    public virtual ICollection<PriceInfo>? PriceInfos { get; set; }
    public virtual ICollection<OrderBook>? OrderBooks { get; set; }
    public virtual ICollection<PublicTrade>? PublicTrades { get; set; }
    public virtual ICollection<InstrumentDetail>? InstrumentDetails { get; set; }
    public virtual ICollection<CorporateAction>? CorporateActions { get; set; }
    public virtual ICollection<TradingSign>? TradingSigns { get; set; }
    public virtual ICollection<Financial>? Financials { get; set; }
    public virtual ICollection<FundPerformance>? FundPerformances { get; set; }
    public virtual ICollection<FundDetail>? FundDetails { get; set; }
    public virtual ICollection<NavList>? NavLists { get; set; }
    public virtual ICollection<FundTradeDate>? FundTradeDates { get; set; }
    public virtual ICollection<Indicator>? Indicators { get; set; }
    public virtual ICollection<Intermission>? Intermissions { get; set; }
}