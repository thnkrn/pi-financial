using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

public class TradeTickerMessageWrapper : ItchMessage
{
    public TradeTickerMessageWrapper()
    {
        MsgType = ItchMessageType.i;
    }

    public Numeric32 Nanos { get; set; }
    public OrderBookId? OrderbookId { get; set; }
    public DealId? DealId { get; set; }
    public DealSource? DealSource { get; set; }
    public Timestamp DealDateTime { get; set; }
    public Action? Action { get; set; }
    public Alpha? Aggressor { get; set; }
    public Numeric64 Quantity { get; set; }
    public Price32 Price { get; set; }
    public TradeReportCode? TradeReportCode { get; set; }
    public Metadata? Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}