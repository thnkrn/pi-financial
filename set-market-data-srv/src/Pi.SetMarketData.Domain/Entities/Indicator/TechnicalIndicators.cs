using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pi.SetMarketData.Domain.Entities;

public class TechnicalIndicators
{
    [Key, Column("date_time", Order = 0)]
    public DateTime? DateTime { get; set; }
    [Key, Column("symbol", Order = 1)]
    public string? Symbol { get; set; }
    [Key, Column("venue", Order = 2)]
    public string? Venue { get; set; }
    [Column("sma10")]
    public double? Sma10 { get; set; }
    [Column("sma25")]
    public double? Sma25 { get; set; }
    [Column("ema10")]
    public double? Ema10 { get; set; }
    [Column("ema25")]
    public double? Ema25 { get; set; }
    [Column("macd_ema12")]
    public double? MacdEma12 { get; set; }
    [Column("macd_ema26")]
    public double? MacdEma26 { get; set; }
    [Column("macd_macd_diff")]
    public double? MacdMacdDiff { get; set; }
    [Column("macd_signal_dea")]
    public double? MacdSignalDea { get; set; }
    [Column("macd_osc")]
    public double? MacdOsc { get; set; }
    [Column("rsi_rsi")]
    public double? RsiRsi { get; set; }
    [Column("rsi_gain_smma_up")]
    public double? RsiGainSmmaUp { get; set; }
    [Column("rsi_loss_smma_down")]
    public double? RsiLossSmmaDown { get; set; }
    [Column("boll_upper")]
    public double? BollUpper { get; set; }
    [Column("boll_medium")]
    public double? BollMedium { get; set; }
    [Column("boll_lower")]
    public double? BollLower { get; set; }
    [Column("kdj_k")]
    public double? KdjK { get; set; }
    [Column("kdj_d")]
    public double? KdjD { get; set; }
    [Column("kdj_j")]
    public double? KdjJ { get; set; }
}

public class TechnicalIndicators1Month : TechnicalIndicators
{
    [NotMapped]
    public readonly string Timeframe = "1 month";
}

public class TechnicalIndicators1Week : TechnicalIndicators
{
    [NotMapped]
    public readonly string Timeframe = "1 week";
}

public class TechnicalIndicators1Day : TechnicalIndicators
{
    [NotMapped]
    public readonly string Timeframe = "1 day";
}

public class TechnicalIndicators4Hour : TechnicalIndicators
{
    [NotMapped]
    public readonly string Timeframe = "4 hour";
}

public class TechnicalIndicators2Hour : TechnicalIndicators
{
    [NotMapped]
    public readonly string Timeframe = "2 hour";
}

public class TechnicalIndicators1Hour : TechnicalIndicators
{
    [NotMapped]
    public readonly string Timeframe = "1 hour";
}

public class TechnicalIndicators30Min : TechnicalIndicators
{
    [NotMapped]
    public readonly string Timeframe = "30 min";
}

public class TechnicalIndicators15Min : TechnicalIndicators
{
    [NotMapped]
    public readonly string Timeframe = "15 min";
}

public class TechnicalIndicators5Min : TechnicalIndicators
{
    [NotMapped]
    public readonly string Timeframe = "5 min";
}

public class TechnicalIndicators2Min : TechnicalIndicators
{
    [NotMapped]
    public readonly string Timeframe = "2 min";
}

public class TechnicalIndicators1Min : TechnicalIndicators
{
    [NotMapped]
    public readonly string Timeframe = "1 min";
}