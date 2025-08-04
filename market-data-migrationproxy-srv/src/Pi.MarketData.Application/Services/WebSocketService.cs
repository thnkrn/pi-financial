using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pi.MarketData.Application.Interfaces;
using Pi.MarketData.Domain.Enums;
using Pi.MarketData.Domain.Models;

namespace Pi.MarketData.Application.Services;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class WebSocketService : IWebSocketService
{
    private readonly ConcurrentDictionary<WebSocket, CancellationTokenSource> _clients = new();
    private readonly ILogger<WebSocketService> _logger;
    private readonly IProxyService _proxyService;
    private readonly bool _websocketEnabled;
    private bool _disposed;

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="proxyService"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public WebSocketService(ILogger<WebSocketService> logger, IProxyService proxyService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _proxyService = proxyService ?? throw new ArgumentNullException(nameof(proxyService));
        _websocketEnabled = false;
    }

    public void AddClient(WebSocket webSocket)
    {
        _clients.TryAdd(webSocket, new CancellationTokenSource());
    }

    public void RemoveClient(WebSocket webSocket)
    {
        if (_clients.TryRemove(webSocket, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    public async Task HandleWebSocketConnection(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _clients[webSocket].Token);
                await ProcessWebSocketMessage(webSocket, result, buffer);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogDebug(ex, "Operation canceled.");
        }
        catch (WebSocketException ex)
        {
            if (ex.Message.Contains("compressed frame when compression is not enabled",
                    StringComparison.OrdinalIgnoreCase))
                _logger.LogDebug(ex, "Received compressed WebSocket frame but compression is not enabled.");
            else if (ex.Message.Equals(
                         "The remote party closed the WebSocket connection without completing the close handshake.",
                         StringComparison.CurrentCultureIgnoreCase))
                _logger.LogDebug(ex, "WebSocket closed.");
            else
                _logger.LogWarning(ex, "WebSocket exception.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown exception.");
        }
        finally
        {
            await CloseWebSocketIfOpen(webSocket);
            RemoveClient(webSocket);
        }
    }

    public async Task SendMessageToClientAsync(WebSocket webSocket, string message)
    {
        if (webSocket.State == WebSocketState.Open)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(
                new ArraySegment<byte>(messageBytes),
                WebSocketMessageType.Text,
                true,
                _clients[webSocket].Token
            );
        }
    }

    public ConcurrentDictionary<WebSocket, CancellationTokenSource> GetAllClients()
    {
        return _clients;
    }

    public async Task CloseAllConnections()
    {
        foreach (var client in _clients.Keys)
            await CloseWebSocketIfOpen(client);
    }

    public RequestPayload? ParseMessage(string message)
    {
        try
        {
            return !IsValidJson(message)
                ? null
                : JsonConvert.DeserializeObject<RequestPayload>(message);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing message: {Message}", ex.Message);
            return null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private async Task ProcessWebSocketMessage(WebSocket webSocket, WebSocketReceiveResult result, byte[] buffer)
    {
        if (result.MessageType == WebSocketMessageType.Close)
        {
            await CloseWebSocketIfOpen(webSocket);
            RemoveClient(webSocket);
        }
        else if (result.MessageType == WebSocketMessageType.Text)
        {
            await HandleTextMessage(webSocket, result, buffer);
        }
    }

    private async Task HandleTextMessage(WebSocket webSocket, WebSocketReceiveResult result, byte[] buffer)
    {
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        var payload = ParseMessage(message);

        _logger.LogDebug("[{Time}], Received client payload", DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"));

        if (payload?.Op == "Subscribe") await ProcessSubscription(webSocket, payload);
    }

    private async Task ProcessSubscription(WebSocket webSocket, RequestPayload payload)
    {
        var (setPayload, gePayload, wsPayload) = _proxyService.DetermineSubscriptionList(payload);
        List<Task> tasks;

        if (_websocketEnabled)
            tasks =
            [
                _proxyService.SubscribeSignalRHub(webSocket, setPayload, SignalRHubEndpoint.Set,
                    _clients[webSocket].Token),
                _proxyService.SubscribeSignalRHub(webSocket, gePayload, SignalRHubEndpoint.Ge,
                    _clients[webSocket].Token),
                _proxyService.SubscribeWebsocket(webSocket, wsPayload,
                    _clients[webSocket].Token)
            ];
        else
            tasks =
            [
                _proxyService.SubscribeSignalRHub(webSocket, setPayload, SignalRHubEndpoint.Set,
                    _clients[webSocket].Token),
                _proxyService.SubscribeSignalRHub(webSocket, gePayload, SignalRHubEndpoint.Ge,
                    _clients[webSocket].Token)
            ];

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Subscription tasks cancelled for client.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during subscription processing.");
            await CloseWebSocketIfOpen(webSocket);
            RemoveClient(webSocket);
        }
    }

    private static bool IsValidJson(string text)
    {
        text = text.Trim();

        try
        {
            JToken.Parse(text);
            return true;
        }
        catch (JsonReaderException)
        {
            return false;
        }
        catch
        {
            return false;
        }
    }

    private static async Task CloseWebSocketIfOpen(WebSocket webSocket)
    {
        if (webSocket.State == WebSocketState.Open)
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                foreach (var cts in _clients.Values)
                {
                    cts.Cancel();
                    cts.Dispose();
                }

                _clients.Clear();
            }

            _disposed = true;
        }
    }
}