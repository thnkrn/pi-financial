using Pi.GlobalMarketDataWSS.Domain.Models.Request;

namespace Pi.GlobalMarketDataWSS.SignalRHub.Interfaces;

public interface IStreamingMarketDataSubscriberGroupFilter : IHostedService
{
    Task UpdateSubscriptionAsync(string connectionId, MarketStreamingRequest request);
    Task RemoveSubscriptionAsync(string connectionId);
    bool IsHealthy();
}