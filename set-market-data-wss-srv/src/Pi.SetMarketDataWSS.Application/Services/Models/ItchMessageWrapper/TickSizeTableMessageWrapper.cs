using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

public class TickSizeTableMessageWrapper : ItchMessage
{
    public TickSizeTableMessageWrapper()
    {
        MsgType = ItchMessageType.L;
    }

    public OrderBookId? OrderBookId { get; set; }
    public Nanos? Nanos { get; set; }
    public Price? TickSize { get; set; }
    public Price? PriceFrom { get; set; }
    public Price? PriceTo { get; set; }
    public Metadata? Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}
