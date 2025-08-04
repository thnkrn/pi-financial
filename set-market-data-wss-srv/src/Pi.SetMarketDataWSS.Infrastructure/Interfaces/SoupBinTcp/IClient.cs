using System.Net;
using Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp;

namespace Pi.SetMarketDataWSS.Infrastructure.Interfaces.SoupBinTcp;

public interface IClient
{
    void Setup(IPAddress ipAddress, int port, int reconnectDelayMs, LoginDetails loginDetails);
    void Start();
    void Shutdown();
    Task Send(byte[] message);
}