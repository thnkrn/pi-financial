using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class ReferencePriceParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderbookId { get; init; }
    public required Numeric8 PriceType { get; init; }
    public required Price32 Price { get; init; }
    public required Timestamp UpdatedTimestamp { get; init; }
}

public class ReferencePriceMessage : ItchMessage
{
    private const char MessageType = 'Q';

    public ReferencePriceMessage(ReferencePriceParams referencePriceParams)
    {
        MsgType = MessageType;
        Nanos = referencePriceParams.Nanos;
        OrderbookId = referencePriceParams.OrderbookId;
        PriceType = referencePriceParams.PriceType;
        Price = referencePriceParams.Price;
        UpdatedTimestamp = referencePriceParams.UpdatedTimestamp;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderbookId { get; }
    public Numeric8 PriceType { get; }
    public Price32 Price { get; }
    public Timestamp UpdatedTimestamp { get; }

    public static ReferencePriceMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 21)
            throw new ArgumentException("Invalid data format for ReferencePriceMessage. Expected at least 21 bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        return new ReferencePriceMessage(
            new ReferencePriceParams
            {
                Nanos = reader.ReadNumeric32(),
                OrderbookId = reader.ReadNumeric32(),
                PriceType = reader.ReadNumeric8(),
                Price = reader.ReadPrice32(),
                UpdatedTimestamp = reader.ReadTimestamp()
            }
        );
    }

    public string ToStringUnitTest()
    {
        return $"""
                ReferencePriceMessage:
                MsgType: {MsgType},
                Nanos: {Nanos},
                OrderbookId: {OrderbookId},
                PriceType: {PriceType},
                Price: {Price},
                UpdatedTimestamp: {UpdatedTimestamp}
                """;
    }
}