using System.Text;

namespace Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

public class Debug : Message
{
    public Debug(string text)
    {
        const char type = '+';
        var payload = type + text;
        Bytes = Encoding.ASCII.GetBytes(payload);
    }

    internal Debug(byte[]? bytes)
    {
        Bytes = bytes;
    }

    public string Text =>
        Bytes != null ? Encoding.ASCII.GetString(Bytes.Skip(1).Take(Length - 1).ToArray()) : string.Empty;
}