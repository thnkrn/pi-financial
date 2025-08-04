using System.Text;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Collections.Concurrent;
using Pi.SetMarketData.MigrationProxy.Models;
using Pi.SetMarketData.MigrationProxy.Enums;
using Pi.SetMarketData.MigrationProxy.Interfaces;

namespace Pi.SetMarketData.MigrationProxy.Services;

public class WebSocketService : IWebSocketService
{
    private readonly ILogger<WebSocketService> _logger;
    private readonly IProxyService _proxyService;
    public WebSocketService
    (
        ILogger<WebSocketService> logger,
        IProxyService proxyService
    )
    {
        _logger = logger;
        _proxyService = proxyService;
    }
    private readonly ConcurrentDictionary<WebSocket, bool> _clients = new ConcurrentDictionary<WebSocket, bool>();

    public void AddClient(WebSocket webSocket)
    {
        _clients.TryAdd(webSocket, true);
    }

    public void RemoveClient(WebSocket webSocket)
    {
        _clients.TryRemove(webSocket, out _);
    }
    public async Task HandleWebSocketConnection(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                // Optionally, check for any incoming messages
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var payload = ParseMessage(message);

                    // Process the payload
                    if ((payload != null) && (payload.Op == "Subscribe"))
                    {

                        var (SETPayload, GEPayload, wsPayload) = _proxyService.DetermineSubscriptionList(payload);

                        var tasks = new List<Task>
                        {
                            Task.Run(async () =>
                            {
                                await _proxyService.SubscribeSignalRHub(webSocket, SETPayload, SignalRHubEndpoint.SET);
                            }),
                            Task.Run(async () =>
                            {
                                await _proxyService.SubscribeSignalRHub(webSocket, GEPayload, SignalRHubEndpoint.GE);
                            }),
                            Task.Run(async () =>
                            {
                                await _proxyService.SubscribeWebsocket(webSocket, wsPayload);
                            })
                        };
                        await Task.WhenAll(tasks);
                    }

                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
            }
        }
        catch (Exception ex) when (ex is WebSocketException or ObjectDisposedException)
        {
            _logger.LogInformation(ex, "WebSocket closed unexpectedly.");
            return;
        }
        finally
        {
            if (webSocket.State != WebSocketState.Closed)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }
    }

    public async Task SendMessageToClientAsync(WebSocket webSocket, string message)
    {
        if (webSocket.State == WebSocketState.Open)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync
            (
                new ArraySegment<byte>(messageBytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }
    }

    public ConcurrentDictionary<WebSocket, bool> GetAllClients() => _clients;

    public async Task CloseAllConnections()
    {
        foreach (var client in _clients.Keys)
        {
            if (client.State == WebSocketState.Open || client.State == WebSocketState.CloseReceived)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server shutting down", CancellationToken.None);
            }
        }
    }

    public RequestPayload? ParseMessage(string message)
    {
        try
        {
            return JsonConvert.DeserializeObject<RequestPayload>(message);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing message: {Message}", ex.Message);
            return null;
        }
    }
}
