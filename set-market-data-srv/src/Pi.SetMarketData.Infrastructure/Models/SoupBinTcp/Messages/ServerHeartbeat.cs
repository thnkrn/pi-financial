using System.Text;

namespace Pi.SetMarketData.Infrastructure.Models.SoupBinTcp.Messages;

public class ServerHeartbeat : Message
{
    public ServerHeartbeat()
    {
        const char type = 'H';
        Bytes = Encoding.ASCII.GetBytes(new[] {type});
    }
        
    internal ServerHeartbeat(byte[] bytes)
    {
        Bytes = bytes;
    }
}