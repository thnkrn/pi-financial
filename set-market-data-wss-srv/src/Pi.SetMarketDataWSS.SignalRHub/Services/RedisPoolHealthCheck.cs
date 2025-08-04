using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.SetMarketDataWSS.SignalRHub.Helpers;

namespace Pi.SetMarketDataWSS.SignalRHub.Services;

/// <summary>
/// Health check for the Redis connection pool
/// </summary>
public class RedisPoolHealthCheck : IHealthCheck
{
    private readonly RedisConnectionPoolManager _poolManager;
    private readonly ILogger<RedisPoolHealthCheck> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="poolManager"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public RedisPoolHealthCheck(
        RedisConnectionPoolManager poolManager,
        ILogger<RedisPoolHealthCheck> logger)
    {
        _poolManager = poolManager ?? throw new ArgumentNullException(nameof(poolManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get a connection from the pool
            var connection = await _poolManager.GetConnectionAsync(cancellationToken);
            
            // Check if the connection is available and connected
            if (connection is not { IsConnected: true })
            {
                return HealthCheckResult.Unhealthy("Redis connection pool is not connected");
            }
            
            // Try to ping Redis to verify the connection is working
            var db = connection.GetDatabase();
            var pingResult = await db.PingAsync();
            
            // Return the connection to the pool
            _poolManager.ReturnConnection(connection);
            
            // Check the ping latency
            if (pingResult.TotalMilliseconds > 500)
            {
                return HealthCheckResult.Degraded(
                    $"Redis connection is slow: {pingResult.TotalMilliseconds}ms latency");
            }
            
            // Add pool statistics to the health check data
            var data = new Dictionary<string, object>
            {
                { "CurrentPoolSize", _poolManager.CurrentPoolSize },
                { "PingLatency", pingResult.TotalMilliseconds }
            };
            
            return HealthCheckResult.Healthy("Redis connection pool is healthy", data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis connection pool health check failed");
            return HealthCheckResult.Unhealthy("Redis connection pool health check failed", ex);
        }
    }
}