using System.Collections.Concurrent;
using System.Net.WebSockets;
using Pi.MarketData.Domain.Models;

namespace Pi.MarketData.Application.Interfaces;

public interface IWebSocketService : IDisposable
{
    void AddClient(WebSocket webSocket);
    void RemoveClient(WebSocket webSocket);
    Task HandleWebSocketConnection(WebSocket webSocket);
    Task SendMessageToClientAsync(WebSocket webSocket, string message);
    ConcurrentDictionary<WebSocket, CancellationTokenSource> GetAllClients();
    Task CloseAllConnections();
    RequestPayload? ParseMessage(string message);
}