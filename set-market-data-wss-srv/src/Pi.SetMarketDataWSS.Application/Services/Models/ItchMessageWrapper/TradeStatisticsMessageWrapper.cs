using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

public class TradeStatisticsMessageWrapper : ItchMessage
{
    public TradeStatisticsMessageWrapper()
    {
        MsgType = ItchMessageType.I;
    }

    public Nanos? Nanos { get; set; }
    public OrderBookId? OrderBookId { get; set; }
    public OpenPrice? OpenPrice { get; set; }
    public HighPrice? HighPrice { get; set; }
    public LowPrice? LowPrice { get; set; }
    public LastPrice? LastPrice { get; set; }
    public LastAuctionPrice? LastAuctionPrice { get; set; }
    public TurnoverQuantity? TurnoverQuantity { get; set; }
    public ReportedTurnoverQuantity? ReportedTurnoverQuantity { get; set; }
    public TurnoverValue? TurnoverValue { get; set; }
    public ReportedTurnoverValue? ReportedTurnoverValue { get; set; }
    public AveragePrice? AveragePrice { get; set; }
    public TotalNumberOfTrades? TotalNumberOfTrades { get; set; }
    public Metadata? Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}