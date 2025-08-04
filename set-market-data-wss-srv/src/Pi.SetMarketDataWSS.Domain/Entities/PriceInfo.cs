namespace Pi.SetMarketDataWSS.Domain.Entities;

public class PriceInfo
{
    public PriceInfo()
    {
    }

    public PriceInfo(string? currency)
    {
        Currency = currency;
    }

    public int PriceInfoId { get; set; }
    public int InstrumentId { get; set; }
    public string? Price { get; set; }
    public string? Currency { get; set; }
    public string? AuctionPrice { get; set; }
    public string? AuctionVolume { get; set; }
    public string? Open { get; set; }
    public string? High24H { get; set; }
    public string? Low24H { get; set; }
    public string? High52W { get; set; }
    public string? Low52W { get; set; }
    public string? PriceChanged { get; set; }
    public string? PriceChangedRate { get; set; }
    public string? Volume { get; set; }
    public string? Amount { get; set; }
    public string? ChangeAmount { get; set; }
    public string? ChangeVolume { get; set; }
    public string? TurnoverRate { get; set; }
    public string? Open2 { get; set; }
    public string? Ceiling { get; set; }
    public string? Floor { get; set; }
    public string? Average { get; set; }
    public string? AverageBuy { get; set; }
    public string? AverageSell { get; set; }
    public string? Aggressor { get; set; }
    public string? PreClose { get; set; }
    public string? Status { get; set; }
    public string? Yield { get; set; }
    public string? Pe { get; set; }
    public string? Pb { get; set; }
    public string? TotalAmount { get; set; }
    public string? TotalAmountK { get; set; }
    public string? TotalVolume { get; set; }
    public string? TotalVolumeK { get; set; }
    public string? TradableEquity { get; set; }
    public string? TradableAmount { get; set; }
    public string? Eps { get; set; }
    public string? LastTrade { get; set; }
    public int ToLastTrade { get; set; }
    public string? Moneyness { get; set; }
    public string? MaturityDate { get; set; }
    public string? ExercisePrice { get; set; }
    public string? IntrinsicValue { get; set; }
    public string? PSettle { get; set; }
    public string? Poi { get; set; }
    public string? Underlying { get; set; }
    public string? Open0 { get; set; }
    public string? Open1 { get; set; }
    public string? Basis { get; set; }
    public string? Settle { get; set; }
    public string? Symbol { get; set; }
    public string? SecurityType { get; set; }
    public long LastPriceTime { get; set; }
    public bool CalculatedPriceChangedRate { get; set; }
    public virtual Instrument Instrument { get; set; }
}