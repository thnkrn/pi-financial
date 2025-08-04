using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class OpenInterestParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderbookId { get; init; }
    public required Numeric64 OpenInterest { get; init; }
    public required Timestamp Timestamp { get; init; }
}

public class OpenInterestMessage : ItchMessage
{
    public OpenInterestMessage(OpenInterestParams openInterestParams)
    {
        MsgType = 'h'; // 'h' – Open Interest Message
        Nanos = openInterestParams.Nanos;
        OrderbookId = openInterestParams.OrderbookId;
        OpenInterest = openInterestParams.OpenInterest;
        Timestamp = openInterestParams.Timestamp;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderbookId { get; }
    public Numeric64 OpenInterest { get; }
    public Timestamp Timestamp { get; }

    public static OpenInterestMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 24)
            throw new ArgumentException("Invalid data format for OpenInterestMessage. Expected at least 24 bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        return new OpenInterestMessage(
            new OpenInterestParams
            {
                Nanos = reader.ReadNumeric32(),
                OrderbookId = reader.ReadNumeric32(),
                OpenInterest = reader.ReadNumeric64(),
                Timestamp = reader.ReadTimestamp()
            }
        );
    }

    public string ToStringUnitTest()
    {
        return $"""
                OpenInterestMessage:
                MsgType: {MsgType}, Nanos: {Nanos}, OrderbookId: {OrderbookId}, OpenInterest: {OpenInterest},
                Timestamp: {Timestamp}
                """;
    }
}