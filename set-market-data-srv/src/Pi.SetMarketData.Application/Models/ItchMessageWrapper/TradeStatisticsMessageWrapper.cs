using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Models.ItchParser;

namespace Pi.SetMarketData.Application.Models.ItchMessageWrapper;

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class TradeStatisticsMessageWrapper : ItchMessage
{
#pragma warning disable CS8618, CS9264
    public TradeStatisticsMessageWrapper()
#pragma warning restore CS8618, CS9264
    {
        MsgType = ItchMessageType.I;
    }

    public Nanos Nanos { get; set; }
    public OrderBookId OrderBookId { get; set; }
    public OpenPrice OpenPrice { get; set; }
    public HighPrice HighPrice { get; set; }
    public LowPrice LowPrice { get; set; }
    public LastPrice LastPrice { get; set; }
    public LastAuctionPrice LastAuctionPrice { get; set; }
    public TurnoverQuantity TurnoverQuantity { get; set; }
    public ReportedTurnoverQuantity ReportedTurnoverQuantity { get; set; }
    public TurnoverValue TurnoverValue { get; set; }
    public ReportedTurnoverValue ReportedTurnoverValue { get; set; }
    public AveragePrice AveragePrice { get; set; }
    public TotalNumberOfTrades TotalNumberOfTrades { get; set; }
    public Metadata Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}

public class AveragePrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class HighPrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class LastAuctionPrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class LastPrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class LowPrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class OpenPrice
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class ReportedTurnoverQuantity
{
    public int Value { get; set; }
}

public class ReportedTurnoverValue
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}

public class TotalNumberOfTrades
{
    public int Value { get; set; }
}

public class TurnoverQuantity
{
    public int Value { get; set; }
}

public class TurnoverValue
{
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
}