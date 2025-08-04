using System.Net;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;

namespace Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;

public interface IClient : IDisposable
{
    ClientState State { get; }
    Task SetupAsync(IPAddress ipAddress, int port, int reconnectDelayMs, LoginDetails loginDetails);
    Task StartAsync(CancellationToken cancellationToken = default);
    Task ShutdownAsync(CancellationToken cancellationToken = default);
    Task ConnectAsync(CancellationToken cancellationToken = default);
    Task SendAsync(byte[]? message, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
}

public enum ClientState
{
    Disconnected,
    Connecting,
    Connected,
    ShuttingDown
}