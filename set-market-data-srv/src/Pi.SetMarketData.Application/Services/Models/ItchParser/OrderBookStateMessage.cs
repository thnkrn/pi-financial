using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser;

public class OrderBookStateMessageParams
{
    public Numeric32 Nanos { get; set; }
    public Numeric32 OrderBookId { get; set; }
    public required Alpha StateName { get; set; }
}

public class OrderBookStateMessage : ItchMessage
{
    public Numeric32 Nanos { get; private set; }
    public Numeric32 OrderBookId { get; private set; }
    public Alpha StateName { get; private set; }

    public OrderBookStateMessage(OrderBookStateMessageParams orderBookStateMessageParams)
    {
        MsgType = ItchMessageType.O;
        Nanos = orderBookStateMessageParams.Nanos;
        OrderBookId = orderBookStateMessageParams.OrderBookId;
        StateName = orderBookStateMessageParams.StateName;
    }

    public static OrderBookStateMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 28) // Expecting exactly 28 bytes for the OrderBookStateMessage
        {
            throw new ArgumentException("Invalid data format for OrderBookStateMessage.");
        }

        var reader = new ItchMessageByteReader(new Memory<byte>(bytes));
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

    public override string ToString()
    {
        return $"OrderBookStateMessage:\n"
            + $"Nanos: {Nanos},\n"
            + $"OrderBookId: {OrderBookId},\n"
            + $"StateName: {StateName},\n";
    }
}
