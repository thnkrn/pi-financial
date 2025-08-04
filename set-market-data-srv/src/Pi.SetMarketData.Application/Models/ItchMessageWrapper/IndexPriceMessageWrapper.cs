using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Services.Types.ItchParser;

namespace Pi.SetMarketData.Application.Models.ItchMessageWrapper;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class IndexPriceMessageWrapper : ItchMessage
{
#pragma warning disable CS8618, CS9264
    public IndexPriceMessageWrapper()
#pragma warning restore CS8618, CS9264
    {
        MsgType = ItchMessageType.J;
    }

    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    public Nanos Nanos { get; set; }
    public OrderBookId OrderBookId { get; set; }
    public Price64 Value { get; set; }
    public Price64 HighValue { get; set; }
    public Price64 LowValue { get; set; }
    public Price64 OpenValue { get; set; }
    public Numeric TradedVolume { get; set; }
    public Price64 TradedValue { get; set; }
    public Price64 Change { get; set; }
    public Price64 ChangePercent { get; set; }
    public Price64 PreviousClose { get; set; }
    public Price64 Close { get; set; }
    public Timestamp Timestamp { get; set; }
}