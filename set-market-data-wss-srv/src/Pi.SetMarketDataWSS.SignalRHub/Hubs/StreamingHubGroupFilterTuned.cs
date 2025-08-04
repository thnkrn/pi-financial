using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using Pi.SetMarketDataWSS.Domain.Models.Request;
using Pi.SetMarketDataWSS.SignalRHub.Interfaces;

namespace Pi.SetMarketDataWSS.SignalRHub.Hubs;

public sealed class StreamingHubGroupFilterTuned : Hub, IAsyncDisposable
{
    private readonly ActivitySource _activitySource;
    private readonly ILogger<StreamingHubGroupFilterTuned> _logger;
    private readonly string _methodName;
    private readonly IStreamingMarketDataSubscriberGroupFilterTuned _subscriber; // Use standard interface

    /// <summary>
    /// 
    /// </summary>
    /// <param name="subscriber"></param>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public StreamingHubGroupFilterTuned(
        IStreamingMarketDataSubscriberGroupFilterTuned subscriber, // Use standard interface
        ILogger<StreamingHubGroupFilterTuned> logger,
        IConfiguration configuration)
    {
        _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _methodName = configuration[ConfigurationKeys.SignalRHubMethodName]
                      ?? throw new ArgumentException($"{ConfigurationKeys.SignalRHubMethodName} is not configured",
                          nameof(configuration));
        _activitySource = new ActivitySource("StreamingHubTuned");
    }

    public async ValueTask DisposeAsync()
    {
        if (_subscriber is IAsyncDisposable disposable) await disposable.DisposeAsync();
    }

    public async Task SubscribeToStreamDataAsync(MarketStreamingRequest marketStreamingRequest)
    {
        ArgumentNullException.ThrowIfNull(marketStreamingRequest);

        using var activity = _activitySource.StartActivity();
        activity?.SetTag("ConnectionId", Context.ConnectionId);

        try
        {
            await _subscriber.UpdateSubscriptionAsync(Context.ConnectionId, marketStreamingRequest);
            await Clients.Caller.SendAsync(_methodName, new { Message = "Subscribed successfully" });
            _logger.LogDebug("Client {ConnectionId} subscribed successfully", Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing client {ConnectionId}", Context.ConnectionId);
            await Clients.Caller.SendAsync(_methodName, new { Error = "Subscription failed" });
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        using var activity = _activitySource.StartActivity();
        activity?.SetTag("ConnectionId", Context.ConnectionId);

        try
        {
            await _subscriber.RemoveSubscriptionAsync(Context.ConnectionId);
            _logger.LogDebug("Client {ConnectionId} disconnected", Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing subscription for client {ConnectionId}", Context.ConnectionId);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        }
        finally
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}