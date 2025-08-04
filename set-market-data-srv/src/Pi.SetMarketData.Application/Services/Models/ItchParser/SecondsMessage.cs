using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Types.ItchParser;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser;

public class SecondsMessage : ItchMessage
{
    public Numeric32 Second { get; private set; }

    private SecondsMessage(Numeric32 second)
    {
        MsgType = ItchMessageType.T;
        Second = second;
    }

    public static SecondsMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 4) // Expecting exactly 4 bytes for the Unix time
        {
            throw new ArgumentException("Invalid data format for SecondsMessage.");
        }

        using (var memoryStream = new MemoryStream(bytes))
        using (var reader = new BinaryReader(memoryStream))
        {
            Numeric32 second = new Numeric32(reader.ReadBytes(4));

            return new SecondsMessage(second);
        }
    }

    public override string ToString()
    {
        return $"SecondsMessage:\n" + $"Second: {Second},\n";
    }
}
