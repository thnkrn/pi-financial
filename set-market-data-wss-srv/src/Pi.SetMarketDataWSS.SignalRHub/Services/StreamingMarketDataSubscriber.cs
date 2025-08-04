using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using Pi.SetMarketDataWSS.Domain.Models.Request;
using Pi.SetMarketDataWSS.Domain.Models.Response;
using Pi.SetMarketDataWSS.SignalRHub.Hubs;
using Pi.SetMarketDataWSS.SignalRHub.Interfaces;
using Polly;
using StackExchange.Redis;

namespace Pi.SetMarketDataWSS.SignalRHub.Services;

public sealed class StreamingMarketDataSubscriber : IStreamingMarketDataSubscriber, IAsyncDisposable
{
    private readonly ActivitySource _activitySource;
    private readonly string _channel;
    private readonly IAsyncPolicy _circuitBreakerPolicy;
    private readonly ConcurrentDictionary<string, MarketStreamingRequest> _connectionRequests;
    private readonly string _groupName;
    private readonly IHubContext<StreamingHub> _hubContext;
    private readonly ILogger<StreamingMarketDataSubscriber> _logger;
    private readonly string _methodName;
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly ConcurrentDictionary<string, HashSet<string>> _symbolSubscriptions;
    private Task? _executingTask;
    private CancellationTokenSource? _stoppingCts;

    public StreamingMarketDataSubscriber(
        IConnectionMultiplexer redisConnection,
        IHubContext<StreamingHub> hubContext,
        IConfiguration configuration,
        ILogger<StreamingMarketDataSubscriber> logger,
        IAsyncPolicy circuitBreakerPolicy)
    {
        _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _circuitBreakerPolicy = circuitBreakerPolicy ?? throw new ArgumentNullException(nameof(circuitBreakerPolicy));
        _connectionRequests = new ConcurrentDictionary<string, MarketStreamingRequest>();
        _symbolSubscriptions = new ConcurrentDictionary<string, HashSet<string>>();

        _groupName = configuration[ConfigurationKeys.SignalRHubGroupName] ??
                     throw new ArgumentNullException(nameof(configuration),
                         $"{ConfigurationKeys.SignalRHubGroupName} is not configured");
        _methodName = configuration[ConfigurationKeys.SignalRHubMethodName] ??
                      throw new ArgumentNullException(nameof(configuration),
                          $"{ConfigurationKeys.SignalRHubMethodName} is not configured");
        _channel = configuration[ConfigurationKeys.RedisChannel] ??
                   throw new ArgumentNullException(nameof(configuration),
                       $"{ConfigurationKeys.RedisChannel} is not configured");

        _activitySource = new ActivitySource("StreamingMarketDataSubscriber");

        if (string.IsNullOrEmpty(_channel))
            throw new ArgumentException("Redis channel cannot be empty", nameof(configuration));

        var keyspace = configuration[ConfigurationKeys.RedisKeyspace] ?? string.Empty;

        if (!string.IsNullOrEmpty(keyspace)) _channel = $"{keyspace}{_channel}";
    }

    public async ValueTask DisposeAsync()
    {
        if (_stoppingCts != null)
            try
            {
                await StopAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during StreamingMarketDataSubscriber disposal");
            }
            finally
            {
                _stoppingCts.Dispose();
                _stoppingCts = null;
            }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("StreamingMarketDataSubscriber is starting.");
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executingTask = ExecuteAsync(_stoppingCts.Token);
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null) return;

        _logger.LogDebug("StreamingMarketDataSubscriber is stopping.");

        try
        {
            if (_stoppingCts is { IsCancellationRequested: false }) await _stoppingCts.CancelAsync();
        }
        finally
        {
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);
        }
    }

    public Task UpdateSubscriptionAsync(string connectionId, MarketStreamingRequest request)
    {
        if (string.IsNullOrEmpty(connectionId))
            throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));

        ArgumentNullException.ThrowIfNull(request);

        var symbols = request.Data?.Param?.Select(p => p.Symbol).Where(s => !string.IsNullOrEmpty(s)).ToHashSet() ?? [];

        _symbolSubscriptions.AddOrUpdate(connectionId, symbols, (_, oldSet) =>
        {
            oldSet.Clear();
            foreach (var symbol in symbols.OfType<string>()) oldSet.Add(symbol);

            return oldSet;
        });

        _connectionRequests[connectionId] = request;
        _logger.LogDebug("Updated subscription for client {ConnectionId}. Symbols: {Symbols}", connectionId,
            string.Join(", ", symbols));

        return Task.CompletedTask;
    }

    public Task RemoveSubscriptionAsync(string connectionId)
    {
        if (string.IsNullOrEmpty(connectionId))
            throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));

        _symbolSubscriptions.TryRemove(connectionId, out _);
        _connectionRequests.TryRemove(connectionId, out _);
        _logger.LogDebug("Removed subscription for client {ConnectionId}", connectionId);

        return Task.CompletedTask;
    }

    public bool IsHealthy()
    {
        return _redisConnection.IsConnected && _executingTask?.IsCompleted == false;
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _redisConnection.GetSubscriber();
        var subPatternChannel = new RedisChannel(_channel, RedisChannel.PatternMode.Pattern);

        _logger.LogDebug("Subscribing to Redis channel: {Channel}", _channel);

        await _circuitBreakerPolicy.ExecuteAsync(async () =>
        {
            await subscriber.SubscribeAsync(subPatternChannel, StreamingResponseHandler);
        });

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("StreamingMarketDataSubscriber is stopping");
        }
    }

    private async void StreamingResponseHandler(RedisChannel channel, RedisValue message)
    {
        using var activity = _activitySource.StartActivity();
        try
        {
            var msg = message.ToString();
            _logger.LogDebug("Received message from Redis: {Message}", msg);

            var marketStreamingResponse = JsonSerializer.Deserialize<MarketStreamingResponse>(msg);
            if (marketStreamingResponse?.Response?.Data != null)
                await SendToMatchingClientsOptimizedAsync(marketStreamingResponse).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Redis message");
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        }
    }

    private async Task SendToMatchingClientsOptimizedAsync(MarketStreamingResponse response)
    {
        using var activity = _activitySource.StartActivity();
        var tasks = new List<Task>();

        if (response.Response?.Data == null) return;

        foreach (var data in response.Response.Data)
        {
            var symbol = data.Symbol;
            if (string.IsNullOrEmpty(symbol)) continue;

            var matchingConnections = _symbolSubscriptions
                .Where(kvp => kvp.Value.Contains(symbol))
                .Select(kvp => kvp.Key)
                .ToList();

            if (matchingConnections.Count == 0) continue;
            var filteredResponse = new MarketStreamingResponse
            {
                Code = response.Code,
                Op = response.Op,
                Message = response.Message,
                Response = new StreamingResponse { Data = new List<StreamingBody> { data } }
            };

            tasks.AddRange(matchingConnections.Select(connectionId =>
                _hubContext.Clients.Client(connectionId).SendAsync(_methodName, filteredResponse)));
        }

        if (tasks.Count != 0)
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
            _logger.LogDebug("Sent updates to {ClientCount} clients", tasks.Count);
        }
    }
}