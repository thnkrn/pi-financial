using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Services.Types.ItchParser;

namespace Pi.SetMarketData.Application.Models.ItchMessageWrapper;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class TradeTickerMessageWrapper : ItchMessage
{
#pragma warning disable CS8618, CS9264
    public TradeTickerMessageWrapper()
#pragma warning restore CS8618, CS9264
    {
        MsgType = ItchMessageType.i;
    }

    public Numeric Nanos { get; set; }
    public Numeric OrderbookId { get; set; }
    public DealId DealId { get; set; }
    public Numeric DealSource { get; set; }
    public Timestamp DealDateTime { get; set; }
    public Numeric Action { get; set; }
    public Aggressor Aggressor { get; set; }
    public Numeric Quantity { get; set; }
    public Price64 Price { get; set; }
    public Numeric TradeReportCode { get; set; }
    public Metadata Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}

public class Aggressor
{
    public string? Value { get; set; }
}