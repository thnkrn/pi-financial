using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

public class ReferencePriceMessageWrapper : ItchMessage
{
    public ReferencePriceMessageWrapper()
    {
        MsgType = ItchMessageType.Q;
    }

    public Nanos? Nanos { get; set; }
    public OrderBookId? OrderBookId { get; set; }
    public Numeric8 PriceType { get; set; }
    public Price32 Price { get; set; }
    public Metadata? Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}