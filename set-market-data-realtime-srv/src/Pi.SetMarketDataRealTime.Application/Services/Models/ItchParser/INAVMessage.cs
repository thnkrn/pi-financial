using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class INAVParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderBookId { get; init; }
    public required Price32 INAV { get; init; }
    public required Price32 Change { get; init; }
    public required Price32 PercentageChange { get; init; }
    public required Timestamp Timestamp { get; init; }
}

public class INAVMessage : ItchMessage
{
    public INAVMessage(INAVParams inavParams)
    {
        MsgType = ItchMessageType.f;
        Nanos = inavParams.Nanos;
        OrderBookId = inavParams.OrderBookId;
        INAV = inavParams.INAV;
        Change = inavParams.Change;
        PercentageChange = inavParams.PercentageChange;
        Timestamp = inavParams.Timestamp;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderBookId { get; }
    public Price32 INAV { get; }
    public Price32 Change { get; }
    public Price32 PercentageChange { get; }
    public Timestamp Timestamp { get; }

    public static INAVMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 28)
            throw new ArgumentException("Invalid data format for INAVMessage. Expected at least 28 bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        return new INAVMessage(new INAVParams
        {
            Nanos = reader.ReadNumeric32(),
            OrderBookId = reader.ReadNumeric32(),
            INAV = reader.ReadPrice32(),
            Change = reader.ReadPrice32(),
            PercentageChange = reader.ReadPrice32(),
            Timestamp = reader.ReadTimestamp()
        });
    }

    public string ToStringUnitTest()
    {
        return $"""
                INAVMessage:
                MsgType: {MsgType},
                Nanos: {Nanos},
                OrderBookId: {OrderBookId},
                INAV: {INAV},
                Change: {Change},
                PercentageChange: {PercentageChange},
                Timestamp: {Timestamp}
                """;
    }
}