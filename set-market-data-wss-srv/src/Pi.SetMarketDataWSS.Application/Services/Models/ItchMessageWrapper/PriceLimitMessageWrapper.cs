using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

public class PriceLimitMessageWrapper : ItchMessage
{
    public PriceLimitMessageWrapper()
    {
        MsgType = ItchMessageType.k;
    }

    public Nanos? Nanos { get; set; }
    public OrderBookId? OrderbookId { get; set; }
    public UpperLimit? UpperLimit { get; set; }
    public LowerLimit? LowerLimit { get; set; }
    public Metadata? Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}