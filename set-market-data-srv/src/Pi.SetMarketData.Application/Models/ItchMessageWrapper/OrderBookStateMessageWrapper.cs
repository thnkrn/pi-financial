using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Models.ItchParser;

namespace Pi.SetMarketData.Application.Models.ItchMessageWrapper;

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class OrderBookStateMessageWrapper : ItchMessage
{
#pragma warning disable CS8618, CS9264
    public OrderBookStateMessageWrapper()
#pragma warning restore CS8618, CS9264
    {
        MsgType = ItchMessageType.O;
    }

    public Nanos Nanos { get; set; }
    public OrderBookId OrderBookId { get; set; }
    public StateName StateName { get; set; }
    public Metadata Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}

public class OrderBookId
{
    public int Value { get; set; }
}

public class StateName
{
    public string? Value { get; set; }
}