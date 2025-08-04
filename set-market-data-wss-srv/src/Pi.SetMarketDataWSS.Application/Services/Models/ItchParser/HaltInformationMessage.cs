using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

public class HaltInformationMessageParams
{
    public Numeric32 Nanos { get; set; }
    public Numeric32 OrderBookId { get; set; }
    public required Alpha InstrumentState { get; set; }
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

    public Numeric32 Nanos { get; }
    public Numeric32 OrderBookId { get; }
    public Alpha InstrumentState { get; }

    public static HaltInformationMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 28) // Expecting exactly 28 bytes for the HaltInformationMessage
            throw new ArgumentException("Invalid data format for HaltInformationMessage.");

        var reader = new ItchMessageByteReader(new Memory<byte>(bytes));
        var nanos = reader.ReadNumeric32();
        var orderBookId = reader.ReadNumeric32();
        var instrumentState = reader.ReadAlpha(20);

        var haltInformationMessageParams = new HaltInformationMessageParams
        {
            Nanos = nanos,
            OrderBookId = orderBookId,
            InstrumentState = instrumentState
        };

        return new HaltInformationMessage(haltInformationMessageParams);
    }

    public string ToStringUnitTest()
    {
        return $"HaltInformationMessage:\n"
               + $"Nanos: {Nanos},\n"
               + $"OrderBookId: {OrderBookId},\n"
               + $"InstrumentState: {InstrumentState},\n";
    }
}