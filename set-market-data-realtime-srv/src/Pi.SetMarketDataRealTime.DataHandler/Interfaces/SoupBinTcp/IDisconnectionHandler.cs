namespace Pi.SetMarketDataRealTime.DataHandler.Interfaces.SoupBinTcp;

public interface IDisconnectionHandler
{
    Task HandleUnexpectedDisconnectionAsync(CancellationToken cancellationToken = default);
    Task HandleLoginRejectedDisconnectionAsync(CancellationToken cancellationToken = default);
}