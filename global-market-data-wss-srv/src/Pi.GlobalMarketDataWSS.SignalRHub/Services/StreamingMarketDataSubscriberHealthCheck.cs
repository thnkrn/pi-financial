using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.GlobalMarketDataWSS.SignalRHub.Interfaces;

namespace Pi.GlobalMarketDataWSS.SignalRHub.Services;

public class StreamingMarketDataSubscriberHealthCheck(IStreamingMarketDataSubscriberGroupFilterTuned subscriber)
    : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var isHealthy = subscriber.IsHealthy();

        return Task.FromResult(isHealthy
            ? HealthCheckResult.Healthy("StreamingMarketDataSubscriberGroupFilterTuned is healthy")
            : HealthCheckResult.Unhealthy("StreamingMarketDataSubscriberGroupFilterTuned is not functioning correctly"));
    }
}