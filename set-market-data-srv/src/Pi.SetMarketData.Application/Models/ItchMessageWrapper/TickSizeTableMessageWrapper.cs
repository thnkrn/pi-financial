using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Services.Types.ItchParser;

namespace Pi.SetMarketData.Application.Models.ItchMessageWrapper;

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class TickSizeTableMessageWrapper : ItchMessage
{
    public TickSizeTableMessageWrapper()
    {
        MsgType = ItchMessageType.L;
    }

    public OrderBookId? OrderBookId { get; set; }
    public Nanos? Nanos { get; set; }
    public Price64? TickSize { get; set; }
    public Price64? PriceFrom { get; set; }
    public Price64? PriceTo { get; set; }
    public Metadata? Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}