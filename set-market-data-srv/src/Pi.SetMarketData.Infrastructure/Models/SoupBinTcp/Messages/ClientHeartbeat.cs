using System.Text;

namespace Pi.SetMarketData.Infrastructure.Models.SoupBinTcp.Messages;

public class ClientHeartbeat : Message
{
    public ClientHeartbeat()
    {
        const char type = 'R';
        Bytes = Encoding.ASCII.GetBytes(new[] {type});
    }

    internal ClientHeartbeat(byte[] bytes)
    {
        Bytes = bytes;
    }
}