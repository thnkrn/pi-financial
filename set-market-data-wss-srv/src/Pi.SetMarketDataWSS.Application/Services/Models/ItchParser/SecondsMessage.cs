using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

public class SecondsMessage : ItchMessage
{
    private SecondsMessage(Numeric32 second)
    {
        MsgType = ItchMessageType.T;
        Second = second;
    }

    public Numeric32 Second { get; }

    public static SecondsMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 4) // Expecting exactly 4 bytes for the Unix time
            throw new ArgumentException("Invalid data format for SecondsMessage.");

        using (var memoryStream = new MemoryStream(bytes))
        using (var reader = new BinaryReader(memoryStream))
        {
            var second = new Numeric32(reader.ReadBytes(4));

            return new SecondsMessage(second);
        }
    }

    public string ToStringUnitTest()
    {
        return $"SecondsMessage:\n" + $"Second: {Second},\n";
    }
}