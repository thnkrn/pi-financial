using Pi.GlobalMarketDataWSS.Domain.Models.Request;

namespace Pi.GlobalMarketDataWSS.SignalRHub.Interfaces;

public interface IStreamingMarketDataSubscriberGroupFilterTuned : IHostedService
{
    Task UpdateSubscriptionAsync(string connectionId, MarketStreamingRequest request);
    Task RemoveSubscriptionAsync(string connectionId);
    bool IsHealthy();
}