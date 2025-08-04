using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.SetMarketDataWSS.SignalRHub.Interfaces;

namespace Pi.SetMarketDataWSS.SignalRHub.Services;

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