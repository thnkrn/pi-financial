namespace Pi.SetMarketDataWSS.Domain.Entities;

public class Indicator
{
    public int IndicatorId { get; set; }
    public int InstrumentId { get; set; }
    public string? CandleType { get; set; }
    public int FirstCandleTime { get; set; }
    public int CandleTime { get; set; }
    public double Open { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Close { get; set; }
    public double Volume { get; set; }
    public double BollUpper { get; set; }
    public double BollMiddle { get; set; }
    public double BollLower { get; set; }
    public double Ema { get; set; }
    public double KdjK { get; set; }
    public double KdjD { get; set; }
    public double KdjJ { get; set; }
    public double Ma { get; set; }
    public double MacdMacd { get; set; }
    public double MacdSignal { get; set; }
    public double MacdHist { get; set; }
    public double Rsi { get; set; }

    public virtual Instrument Instrument { get; set; }
}