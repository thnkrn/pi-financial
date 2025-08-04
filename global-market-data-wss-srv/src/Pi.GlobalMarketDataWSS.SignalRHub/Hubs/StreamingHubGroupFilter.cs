using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Pi.GlobalMarketDataWSS.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataWSS.Domain.Models.Request;
using Pi.GlobalMarketDataWSS.SignalRHub.Interfaces;

namespace Pi.GlobalMarketDataWSS.SignalRHub.Hubs;

public sealed class StreamingHubGroupFilter : Hub, IAsyncDisposable
{
    private readonly ActivitySource _activitySource;
    private readonly ILogger<StreamingHubGroupFilter> _logger;
    private readonly string _methodName;
    private readonly IStreamingMarketDataSubscriberGroupFilter _subscriber;

    /// <inheritdoc />
    public StreamingHubGroupFilter(IStreamingMarketDataSubscriberGroupFilter subscriber,
        ILogger<StreamingHubGroupFilter> logger,
        IConfiguration configuration)
    {
        _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _methodName = configuration[ConfigurationKeys.SignalRHubMethodName]
                      ?? throw new ArgumentException($"{ConfigurationKeys.SignalRHubMethodName} is not configured",
                          nameof(configuration));
        _activitySource = new ActivitySource("StreamingHubNoFilter");
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