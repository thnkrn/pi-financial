using QuickFix;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Fix;

public interface IClient : IDisposable
{
    ClientState State { get; }
    void Setup(string configFile);
    Task StartAsync(CancellationToken cancellationToken = default);
    Task ShutdownAsync(CancellationToken cancellationToken = default);
    Task Reset(CancellationToken cancellationToken = default);
    Task ConnectAsync(CancellationToken cancellationToken = default);
    Task SendAsync(Message message, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
    Task<bool> CheckListenerSession();
}

public enum ClientState
{
    Disconnected,
    Connecting,
    Connected,
    ShuttingDown
}