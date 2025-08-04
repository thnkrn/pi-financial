using Pi.SetMarketDataWSS.Domain.Models.Request;

namespace Pi.SetMarketDataWSS.SignalRHub.Interfaces;

public interface IStreamingMarketDataSubscriber : IHostedService
{
    Task UpdateSubscriptionAsync(string connectionId, MarketStreamingRequest request);
    Task RemoveSubscriptionAsync(string connectionId);
    bool IsHealthy();
}