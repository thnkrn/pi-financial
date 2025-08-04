using System.Text;

namespace Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp.Messages;

public class LogoutRequest : Message
{
    public LogoutRequest()
    {
        const char type = 'O';
        Bytes = Encoding.ASCII.GetBytes(new[] { type });
    }

    internal LogoutRequest(byte[] bytes)
    {
        Bytes = bytes;
    }
}