using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Skender.Stock.Indicators;

namespace Pi.SetMarketData.Domain.Entities;

public static class CandleType
{
    public const string candle1Min = "1min";
    public const string candle2Min = "2min";
    public const string candle5Min = "5min";
    public const string candle15Min = "15min";
    public const string candle30Min = "30min";
    public const string candle60Min = "60min";
    public const string candle120Min = "120min";
    public const string candle240Min = "240min";
    public const string candle1Day = "1day";
    public const string candle1Week = "1week";
    public const string candle1Month = "1month";

    public static List<string> GetTimeFrames() =>
        [
            candle1Min,
            candle2Min,
            candle5Min,
            candle15Min,
            candle30Min,
            candle60Min,
            candle120Min,
            candle240Min,
            candle1Day,
            candle1Week,
            candle1Month
        ];
}

public class CandleData : IQuote
{
    [Key, Column("bucket", Order = 0)]
    public DateTime Date { get; set; }

    [Key, Column("symbol", Order = 1)]
    public string? Symbol { get; set; }

    [Key, Column("venue", Order = 2)]
    public string? Venue { get; set; }

    [Column("open")]
    public double OpenDouble { get; set; }

    [Column("high")]
    public double HighDouble { get; set; }

    [Column("low")]
    public double LowDouble { get; set; }

    [Column("close")]
    public double CloseDouble { get; set; }

    [Column("volume")]
    public double VolumeDouble { get; set; }

    [Column("amount")]
    public double AmountDouble { get; set; }

    [NotMapped]
    public decimal Open => (decimal)OpenDouble;

    [NotMapped]
    public decimal High => (decimal)HighDouble;

    [NotMapped]
    public decimal Low => (decimal)LowDouble;

    [NotMapped]
    public decimal Close => (decimal)CloseDouble;

    [NotMapped]
    public decimal Volume => (decimal)VolumeDouble;

    [NotMapped]
    public decimal Amount => (decimal)AmountDouble;
}

public class Candle1Min : CandleData
{
    [NotMapped]
    public readonly string Timeframe = CandleType.candle1Min;
}

public class Candle2Min : CandleData
{
    [NotMapped]
    public readonly string Timeframe = CandleType.candle2Min;
}


public class Candle5Min : CandleData
{
    [NotMapped]
    public readonly string Timeframe = CandleType.candle5Min;
}

public class Candle15Min : CandleData
{
    [NotMapped]
    public readonly string Timeframe = CandleType.candle15Min;
}

public class Candle30Min : CandleData
{
    [NotMapped]
    public readonly string Timeframe = CandleType.candle30Min;
}

public class Candle1Hour : CandleData
{
    [NotMapped]
    public readonly string Timeframe = CandleType.candle60Min;
}

public class Candle2Hour : CandleData
{
    [NotMapped]
    public readonly string Timeframe = CandleType.candle120Min;
}

public class Candle4Hour : CandleData
{
    [NotMapped]
    public readonly string Timeframe = CandleType.candle240Min;
}

public class Candle1Day : CandleData
{
    [NotMapped]
    public readonly string Timeframe = CandleType.candle1Day;
}

public class Candle1Week : CandleData
{
    [NotMapped]
    public readonly string Timeframe = CandleType.candle1Week;
}

public class Candle1Month : CandleData
{
    [NotMapped]
    public readonly string Timeframe = CandleType.candle1Month;
}
