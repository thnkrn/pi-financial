using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class OrderBookStateMessageParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderBookId { get; init; }
    public required Alpha StateName { get; init; }
}

public class OrderBookStateMessage : ItchMessage
{
    public OrderBookStateMessage(OrderBookStateMessageParams orderBookStateMessageParams)
    {
        MsgType = ItchMessageType.O;
        Nanos = orderBookStateMessageParams.Nanos;
        OrderBookId = orderBookStateMessageParams.OrderBookId;
        StateName = orderBookStateMessageParams.StateName;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderBookId { get; }
    public Alpha StateName { get; }

    public static OrderBookStateMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length != 28)
            throw new ArgumentException("Invalid data format for OrderBookStateMessage. Expected exactly 28 bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        var nanos = reader.ReadNumeric32();
        var orderBookId = reader.ReadNumeric32();
        var stateName = reader.ReadAlpha(20);

        var orderBookStateMessageParams = new OrderBookStateMessageParams
        {
            Nanos = nanos,
            OrderBookId = orderBookId,
            StateName = stateName
        };

        return new OrderBookStateMessage(orderBookStateMessageParams);
    }

    public string ToStringUnitTest()
    {
        return $"""
                OrderBookStateMessage:
                Nanos: {Nanos},
                OrderBookId: {OrderBookId},
                StateName: {StateName}
                """;
    }
}