using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class SecondsMessage : ItchMessage
{
    public SecondsMessage(Numeric32 second)
    {
        MsgType = ItchMessageType.T;
        Second = second;
    }

    public Numeric32 Second { get; }

    public static SecondsMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        if (bytes.Length != 4 && bytes.Length != 5)
            throw new ArgumentException("Invalid data format for SecondsMessage. Expected 4 or 5 bytes.",
                nameof(bytes));

        var startIndex = bytes.Length == 5 ? 1 : 0;

        if (bytes.Length == 5 && bytes[0] != (byte)ItchMessageType.T)
            throw new ArgumentException(
                $"Invalid message type for SecondsMessage. Expected {ItchMessageType.T}, got {(char)bytes[0]}.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(new Memory<byte>(bytes, startIndex, 4));
        var second = reader.ReadNumeric32();

        return new SecondsMessage(second);
    }

    public override string ToString()
    {
        return $"{MsgType}\n{Second.Value}";
    }

    public string ToStringUnitTest()
    {
        return $"SecondsMessage: Second: {Second}";
    }
}