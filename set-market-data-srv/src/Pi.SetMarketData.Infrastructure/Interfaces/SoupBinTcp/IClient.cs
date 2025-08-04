using System.Net;
using Pi.SetMarketData.Infrastructure.Models.SoupBinTcp;

namespace Pi.SetMarketData.Infrastructure.Interfaces.SoupBinTcp;

public interface IClient
{
    void Setup(IPAddress ipAddress, int port, int reconnectDelayMs, LoginDetails loginDetails);
    void Start();
    void Shutdown();
    Task Send(byte[] message);
}