using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class EndOfSnapshotMessageParams
{
    public required Alpha SequenceNumber { get; init; }
}

public class EndOfSnapshotMessage : ItchMessage
{
    public EndOfSnapshotMessage(EndOfSnapshotMessageParams endOfSnapshotMessageParams)
    {
        MsgType = 'G';
        SequenceNumber = endOfSnapshotMessageParams.SequenceNumber;
    }

    public Alpha SequenceNumber { get; }

    public static EndOfSnapshotMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        if (bytes.Length != 20)
            throw new ArgumentException("Invalid data format for EndOfSnapshotMessage. Expected 20 bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        var sequenceNumber = reader.ReadAlpha(20);

        return new EndOfSnapshotMessage(new EndOfSnapshotMessageParams
        {
            SequenceNumber = sequenceNumber
        });
    }

    public string ToStringUnitTest()
    {
        return $"EndOfSnapshotMessage:\nSequenceNumber: {SequenceNumber}";
    }
}