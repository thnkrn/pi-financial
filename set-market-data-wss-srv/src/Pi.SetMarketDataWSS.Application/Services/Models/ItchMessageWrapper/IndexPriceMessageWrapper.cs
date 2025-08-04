using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

public class IndexPriceMessageWrapper : ItchMessage
{
    public IndexPriceMessageWrapper()
    {
        MsgType = ItchMessageType.J;
    }

    public Nanos? Nanos { get; set; }
    public OrderBookId? OrderBookId { get; set; }
    public Price64 Value { get; set; }
    public Price64 HighValue { get; set; }
    public Price64 LowValue { get; set; }
    public Price64 OpenValue { get; set; }
    public Numeric64 TradedVolume { get; set; }
    public Price64 TradedValue { get; set; }
    public Price64 Change { get; set; }
    public Price32 ChangePercent { get; set; }
    public Price64 PreviousClose { get; set; }
    public Price64 Close { get; }
    public Timestamp Timestamp { get; }
    public Metadata? Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}