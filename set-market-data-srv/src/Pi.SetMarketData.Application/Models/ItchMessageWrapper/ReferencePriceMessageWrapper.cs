using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Services.Types.ItchParser;

namespace Pi.SetMarketData.Application.Models.ItchMessageWrapper;

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class ReferencePriceMessageWrapper : ItchMessage
{
#pragma warning disable CS8618, CS9264
    public ReferencePriceMessageWrapper()
#pragma warning restore CS8618, CS9264
    {
        MsgType = ItchMessageType.Q;
    }

    public Nanos Nanos { get; set; }
    public OrderBookId OrderBookId { get; set; }
    public Numeric8 PriceType { get; set; }
    public Price64 Price { get; set; }
    public Metadata Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}