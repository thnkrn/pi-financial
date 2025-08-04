using System.Net.WebSockets;
using System.Collections.Concurrent;
using Pi.SetMarketData.MigrationProxy.Models;

namespace Pi.SetMarketData.MigrationProxy.Interfaces;

public interface IWebSocketService
{
    void AddClient(WebSocket webSocket);
    void RemoveClient(WebSocket webSocket);
    Task HandleWebSocketConnection(WebSocket webSocket);
    Task SendMessageToClientAsync(WebSocket webSocket, string message);
    ConcurrentDictionary<WebSocket, bool> GetAllClients();
    Task CloseAllConnections();
    RequestPayload? ParseMessage(string message);
}