using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class HaltInformationMessageParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderBookId { get; init; }
    public required Alpha InstrumentState { get; init; }
}

public class HaltInformationMessage : ItchMessage
{
    public HaltInformationMessage(HaltInformationMessageParams haltInformationMessageParams)
    {
        MsgType = ItchMessageType.l;
        Nanos = haltInformationMessageParams.Nanos;
        OrderBookId = haltInformationMessageParams.OrderBookId;
        InstrumentState = haltInformationMessageParams.InstrumentState;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderBookId { get; }
    public Alpha InstrumentState { get; }

    public static HaltInformationMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length != 28)
            throw new ArgumentException("Invalid data format for HaltInformationMessage. Expected 28 bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        var nanos = reader.ReadNumeric32();
        var orderBookId = reader.ReadNumeric32();
        var instrumentState = reader.ReadAlpha(20);

        return new HaltInformationMessage(new HaltInformationMessageParams
        {
            Nanos = nanos,
            OrderBookId = orderBookId,
            InstrumentState = instrumentState
        });
    }

    public string ToStringUnitTest()
    {
        return $"""
                HaltInformationMessage:
                Nanos: {Nanos},
                OrderBookId: {OrderBookId},
                InstrumentState: {InstrumentState}
                """;
    }
}