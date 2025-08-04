using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

public class OpenInterestMessageWrapper : ItchMessage
{
    public OpenInterestMessageWrapper()
    {
        MsgType = ItchMessageType.h;
    }

    public Nanos? Nanos { get; set; }
    public OrderBookId? OrderBookId { get; set; }
    public Numeric16 OpenInterest { get; set; }
    public Timestamp Timestamp { get; set; }
    public Metadata? Metadata { get; set; }
}